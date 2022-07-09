namespace MyGardenPatch.Commands
{
    public class CommandValidatorNotFoundException<TCommand> : Exception
    {
        public override string Message => $"No command validator found for {typeof(TCommand).FullName}";
    }
}
