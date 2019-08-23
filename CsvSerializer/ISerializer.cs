using System.Text;

namespace Csv {
	public interface ISerializer {
		void SerializeHeader(StringBuilder stringBuilder, char separator);
		void SerializeItem(StringBuilder stringBuilder, object item, char separator);
	}
}
