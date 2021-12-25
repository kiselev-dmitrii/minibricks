using System;
using System.Collections.Generic;
using UnityEngine;

namespace NData
{
    public interface IBinding
    {
        void OnContextChange();
        IList<string> ReferencedPaths { get; }
    }
	
	public interface IBindingPathTarget
	{
	}
	
	public interface IContext
	{
		Delegate FindCommand(string path, IBinding binding);
        Delegate FindParameterizedCommand(string path, IBinding binding);
		Property<T> FindProperty<T>(string path, IBinding binding);
		Property<int> FindEnumProperty(string path, IBinding binding);
		Collection FindCollection(string path, IBinding binding);
	    Context FindContext(string path, IBinding binding);
	}

    public delegate void Command();
    public delegate void ParameterizedCommand(object obj);
	
    public class Context : IBindingPathTarget, IContext
    {
        public const string VariableContextPostfix = "EzVariableContext";
		
		protected void Register(string name, IBindingPathTarget target)
		{
		}
		
		protected void Register(string name, Command target)
		{
		}
		
		
        internal static Delegate NodeToDelegate(object node, string path)
        {
            if (node == null)
                return null;

            var reflectionProperty = ReflectionUtils.GetMethod(node.GetType(), path);

            if (reflectionProperty == null)
            {
                return null;
            }

            return ReflectionUtils.CreateDelegate(typeof(Command), node, reflectionProperty);
        }

        internal static Delegate NodeToParametrizedCommand(object node, string path) {
            if (node == null)
                return null;

            var reflectionProperty = ReflectionUtils.GetMethod(node.GetType(), path);

            if (reflectionProperty == null) {
                return null;
            }

            return ReflectionUtils.CreateDelegate(typeof(ParameterizedCommand), node, reflectionProperty);
        }

        internal static Property<int> NodeToEnumProperty(object node, string path)
        {
            if (node == null)
                return null;

            var reflectionProperty = ReflectionUtils.GetProperty(node.GetType(), path + "Property");
            if (reflectionProperty != null)
            {
                return reflectionProperty.GetValue(node, null) as Property<int>;
            }

            reflectionProperty = ReflectionUtils.GetProperty(node.GetType(), path);
            if (reflectionProperty != null)
            {
                var value = reflectionProperty.GetValue(node, null);
                if (value != null)
                {
                    if (ReflectionUtils.IsEnum(value.GetType()))
                    {
                        return new Property<int>((int)value);
                    }
                }
            }
            return null;
        }

        internal static Collection NodeToCollection(object node, string path)
        {
            if (node == null)
                return null;

            var reflectionProperty = ReflectionUtils.GetProperty(node.GetType(), path);

            if (reflectionProperty == null)
                return null;

            return reflectionProperty.GetValue(node, null) as Collection;
        }

        internal static Context NodeToContext(object node, string path) {
            if (node == null)
                return null;

            var reflectionProperty = ReflectionUtils.GetProperty(node.GetType(), path);

            if (reflectionProperty == null)
                return null;

            return reflectionProperty.GetValue(node, null) as Context;
        }

        internal delegate T Converter<out T>(object node, string leafPropertyName);

        // Changes made in this function should be reflected in FindProperty<T>, see details there
        //
        internal static T Find<T>(object node, string path, IBinding binding, Converter<T> converter)
        {
            if (node == null)
                return default(T);
            
            var pointPos = path.IndexOf('.');
            if (pointPos < 0)
            {
                return converter(node, path);
            }
            
            var nodePropertyName = path.Substring(0, pointPos);
            var pathRest = path.Substring(pointPos + 1);

            var context = node as Context;
            if (context != null)
            {
                context.AddBindingDependency(binding);
            }

            int collectionItemIndex;
            if (int.TryParse(nodePropertyName, out collectionItemIndex) && node is Collection)
            {
                var collection = (Collection)node;
                var varSubNode = collection.GetItemPlaceholder(collectionItemIndex);
                varSubNode.AddBindingDependency(binding);
                var valueReflectionProperty = ReflectionUtils.GetProperty(varSubNode.GetType(), "Value");
                var valueSubNode = valueReflectionProperty.GetValue(varSubNode, null);
                return (valueSubNode == null) ? default(T) : Find(valueSubNode, pathRest, binding, converter);
            }
            
            var varReflectionProperty = ReflectionUtils.GetField(node.GetType(), nodePropertyName + VariableContextPostfix);
            if (varReflectionProperty != null)
            {
                var varSubNode = varReflectionProperty.GetValue(node) as VariableContext;
                if (varSubNode != null)
                {
                    varSubNode.AddBindingDependency(binding);
                    var valueReflectionProperty = ReflectionUtils.GetProperty(varSubNode.GetType(), "Value");
                    var valueSubNode = valueReflectionProperty.GetValue(varSubNode, null);
                    return (valueSubNode == null) ? default(T) : Find(valueSubNode, pathRest, binding, converter);
                }
            }

            var reflectionProperty = ReflectionUtils.GetProperty(node.GetType(), nodePropertyName);
            var subNode = (reflectionProperty == null) ? null : reflectionProperty.GetValue(node, null);
			if (subNode == null)
			{
				var nodeReflectionField = ReflectionUtils.GetField(node.GetType(), nodePropertyName);
				subNode = (nodeReflectionField == null) ? null : nodeReflectionField.GetValue(node);
			}
			return (subNode == null) ? default(T) : Find(subNode, pathRest, binding, converter);
        }
        

        // Copy-paste from Find<T> function
        //
        // Reason:
        // When used in Unity3D for iOS builds and launched on actual device (working however
        // completely fine in emulator), internal calls to generic NodeToProperty converter were
        // causing exceptions, because NodeToProperty<T> with specific types wasn't used explicitly
        // anywhere, but only from within Find<T>. It could be worked around be exposing NodeToProperty
        // to public and explicitly used with types required by user application in some dummy function,
        // as described here http://docs.unity3d.com/Documentation/Manual/TroubleShooting.html
        // Another option is to have dedicated function for properties search with inlined converter.
        // 
        // Pros:
        // - works fine in Unity3D iOS builds
        // - no workarounds required from library users
        // 
        // Cons:
        // - whenever changes are made into path resolve sequence, they have to be made in two places
        //
		internal static Property<T> FindProperty<T>(object node, string path, IBinding binding)
        {
            if (node == null)
                return default(Property<T>);
            
            var pointPos = path.IndexOf('.');
            if (pointPos < 0)
            {
                var reflectionProperty = ReflectionUtils.GetProperty(node.GetType(), path + "Property");
	            if (reflectionProperty != null)
	            {
	                return reflectionProperty.GetValue(node, null) as Property<T>;
	            }

                reflectionProperty = ReflectionUtils.GetProperty(node.GetType(), path);
	            if (reflectionProperty != null)
	            {
	                var value = reflectionProperty.GetValue(node, null);
	                if (value != null)
	                {
	                    if (value is T)
	                    {
	                        return new Property<T>((T) value);
	                    }
	                }
	            }

				var reflectionField = ReflectionUtils.GetField(node.GetType(), path);
				if (reflectionField != null)
				{
					var value = reflectionField.GetValue(node);
					if (value != null)
					{
						if (value is T)
						{
							return new Property<T>((T) value);
						}
					}
				}
	            return null;
            }
            
            var nodePropertyName = path.Substring(0, pointPos);
            var pathRest = path.Substring(pointPos + 1);

		    var context = node as Context;
		    if (context != null)
		    {
		        context.AddBindingDependency(binding);
		    }

            int collectionItemIndex;
            if (int.TryParse(nodePropertyName, out collectionItemIndex) && node is Collection)
            {
                var collection = (Collection)node;
                var varSubNode = collection.GetItemPlaceholder(collectionItemIndex);
                varSubNode.AddBindingDependency(binding);
                var valueReflectionProperty = ReflectionUtils.GetProperty(varSubNode.GetType(), "Value");
                var valueSubNode = valueReflectionProperty.GetValue(varSubNode, null);
                return (valueSubNode == null) ? default(Property<T>) : FindProperty<T>(valueSubNode, pathRest, binding);
            }
            
            var varReflectionProperty = ReflectionUtils.GetField(node.GetType(), nodePropertyName + VariableContextPostfix);
            if (varReflectionProperty != null)
            {
                var varSubNode = varReflectionProperty.GetValue(node) as VariableContext;
                if (varSubNode != null)
                {
                    varSubNode.AddBindingDependency(binding);
                    var valueReflectionProperty = ReflectionUtils.GetProperty(varSubNode.GetType(), "Value");
                    var valueSubNode = valueReflectionProperty.GetValue(varSubNode, null);
                    return (valueSubNode == null) ? default(Property<T>) : FindProperty<T>(valueSubNode, pathRest, binding);
                }
            }

            var nodeReflectionProperty = ReflectionUtils.GetProperty(node.GetType(), nodePropertyName);
            var subNode = (nodeReflectionProperty == null) ? null : nodeReflectionProperty.GetValue(node, null);
			if (subNode == null)
			{
				var nodeReflectionField = ReflectionUtils.GetField(node.GetType(), nodePropertyName);
				subNode = (nodeReflectionField == null) ? null : nodeReflectionField.GetValue(node);
			}
            return (subNode == null) ? default(Property<T>) : FindProperty<T>(subNode, pathRest, binding);
        }

        internal static Property FindProperty(object node, string path, IBinding binding) {
            if (node == null)return null;

            var pointPos = path.IndexOf('.');
            if (pointPos < 0) {
                var reflectionProperty = ReflectionUtils.GetProperty(node.GetType(), path + "Property");
                if (reflectionProperty != null) {
                    return reflectionProperty.GetValue(node, null) as Property;
                } else {
                    return null;
                }
            }

            var nodePropertyName = path.Substring(0, pointPos);
            var pathRest = path.Substring(pointPos + 1);

            var context = node as Context;
            if (context != null) {
                context.AddBindingDependency(binding);
            }

            int collectionItemIndex;
            if (int.TryParse(nodePropertyName, out collectionItemIndex) && node is Collection) {
                var collection = (Collection)node;
                var varSubNode = collection.GetItemPlaceholder(collectionItemIndex);
                varSubNode.AddBindingDependency(binding);
                var valueReflectionProperty = ReflectionUtils.GetProperty(varSubNode.GetType(), "Value");
                var valueSubNode = valueReflectionProperty.GetValue(varSubNode, null);
                return (valueSubNode == null) ? null : FindProperty(valueSubNode, pathRest, binding);
            }

            var varReflectionProperty = ReflectionUtils.GetField(node.GetType(), nodePropertyName + VariableContextPostfix);
            if (varReflectionProperty != null) {
                var varSubNode = varReflectionProperty.GetValue(node) as VariableContext;
                if (varSubNode != null) {
                    varSubNode.AddBindingDependency(binding);
                    var valueReflectionProperty = ReflectionUtils.GetProperty(varSubNode.GetType(), "Value");
                    var valueSubNode = valueReflectionProperty.GetValue(varSubNode, null);
                    return (valueSubNode == null) ? null: FindProperty(valueSubNode, pathRest, binding);
                }
            }

            var nodeReflectionProperty = ReflectionUtils.GetProperty(node.GetType(), nodePropertyName);
            var subNode = (nodeReflectionProperty == null) ? null : nodeReflectionProperty.GetValue(node, null);
            if (subNode == null) {
                var nodeReflectionField = ReflectionUtils.GetField(node.GetType(), nodePropertyName);
                subNode = (nodeReflectionField == null) ? null : nodeReflectionField.GetValue(node);
            }
            return (subNode == null) ? null : FindProperty(subNode, pathRest, binding);
        }


        public Delegate FindCommand(string path, IBinding binding)
        {
// Explicit generic arguments are required here for Mono builds
// ReSharper disable RedundantTypeArgumentsOfMethod
            return Find<Delegate>(this, path, binding, NodeToDelegate);
// ReSharper restore RedundantTypeArgumentsOfMethod
        }

        public Delegate FindParameterizedCommand(string path, IBinding binding) {
            // Explicit generic arguments are required here for Mono builds
            // ReSharper disable RedundantTypeArgumentsOfMethod
            return Find<Delegate>(this, path, binding, NodeToParametrizedCommand);
            // ReSharper restore RedundantTypeArgumentsOfMethod
        }

        public Property<T> FindProperty<T>(string path, IBinding binding)
        {
            return FindProperty<T>(this, path, binding);
        }

        public Property FindProperty(string path, IBinding binding) {
            return FindProperty(this, path, binding);
        }

        public Property<int> FindEnumProperty(string path, IBinding binding)
        {
// Explicit generic arguments are required here for Mono builds
// ReSharper disable RedundantTypeArgumentsOfMethod
            return Find<Property<int>>(this, path, binding, NodeToEnumProperty);
// ReSharper restore RedundantTypeArgumentsOfMethod
        }

        public Collection FindCollection(string path, IBinding binding)
        {
// Explicit generic arguments are required here for Mono builds
// ReSharper disable RedundantTypeArgumentsOfMethod
            return Find<Collection>(this, path, binding, NodeToCollection);
// ReSharper restore RedundantTypeArgumentsOfMethod
        }

        public Context FindContext(string path, IBinding binding) {
            // Explicit generic arguments are required here for Mono builds
            // ReSharper disable RedundantTypeArgumentsOfMethod
            return Find<Context>(this, path, binding, NodeToContext);
            // ReSharper restore RedundantTypeArgumentsOfMethod
        }


        protected virtual void AddBindingDependency(IBinding binding)
        {

        }
    }
	
	public class MonoBehaviourContext : MonoBehaviour, IContext
	{
		public Delegate FindCommand(string path, IBinding binding)
        {
// Explicit generic arguments are required here for Mono builds
// ReSharper disable RedundantTypeArgumentsOfMethod
            return Context.Find<Delegate>(this, path, binding, Context.NodeToDelegate);
// ReSharper restore RedundantTypeArgumentsOfMethod
        }

        public Delegate FindParameterizedCommand(string path, IBinding binding) {
            // Explicit generic arguments are required here for Mono builds
            // ReSharper disable RedundantTypeArgumentsOfMethod
            return Context.Find<Delegate>(this, path, binding, Context.NodeToDelegate);
            // ReSharper restore RedundantTypeArgumentsOfMethod
        }

        public Property<T> FindProperty<T>(string path, IBinding binding)
        {
            return Context.FindProperty<T>(this, path, binding);
        }

        public Property<int> FindEnumProperty(string path, IBinding binding)
        {
// Explicit generic arguments are required here for Mono builds
// ReSharper disable RedundantTypeArgumentsOfMethod
            return Context.Find<Property<int>>(this, path, binding, Context.NodeToEnumProperty);
// ReSharper restore RedundantTypeArgumentsOfMethod
        }

        public Collection FindCollection(string path, IBinding binding)
        {
// Explicit generic arguments are required here for Mono builds
// ReSharper disable RedundantTypeArgumentsOfMethod
            return Context.Find<Collection>(this, path, binding, Context.NodeToCollection);
// ReSharper restore RedundantTypeArgumentsOfMethod
        }


        public Context FindContext(string path, IBinding binding) {
            return Context.Find<Context>(this, path, binding, Context.NodeToContext);
	    }
	}
}
