namespace NData {
	public class CommandBinding : Binding {
		protected System.Delegate _command;
	
		protected override void Unbind()
		{
			base.Unbind();
		
			_command = null;
		}
	
		protected override void Bind()
		{
			base.Bind();
		
			var context = GetContext(Path);
			if (context == null)
				return;
		
			_command = context.FindCommand(Path, this);
		}
	}
}