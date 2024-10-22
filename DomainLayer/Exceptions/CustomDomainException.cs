namespace DomainLayer.Exceptions {
	public class CustomDomainException : Exception {
		public CustomDomainException() : base() { }

		public CustomDomainException(string message) : base(message) { }

		public CustomDomainException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}
