namespace Csv {
	public interface IDeserializer {
		object DeserializeItem(string line, char separator);
	}
}
