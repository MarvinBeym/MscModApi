using System.Collections.Generic;
using Newtonsoft.Json;

namespace MscPartApi
{
	[JsonObject(MemberSerialization.OptIn)]
	internal class PartSave
	{
		[JsonProperty]
		public bool installed = false;

		[JsonProperty]
		public List<Screw> screws = new List<Screw>();
	}
}