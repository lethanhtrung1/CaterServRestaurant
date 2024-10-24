namespace ApplicationLayer.Logging {
	public interface ILogException {
		void LogExceptions(Exception ex);
		void LogExceptions(string message);
		void LogToFile(string message);
		void LogToConsole(string message);
		void LogToDebugger(string message);
	}
}
