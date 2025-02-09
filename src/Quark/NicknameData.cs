using StorySystem;
using SerializableAttribute = System.SerializableAttribute;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace LimbusCompanyPortuguêsBrasileiro
{
    [Serializable]
    public class NicknameData : ScenarioAssetData
    {
        [JsonPropertyName("brname")]
        public string? brname;

        [JsonPropertyName("brNickName")]
        public string? brNickName;

        public NicknameData() { }

        internal static NicknameData Create(ref Utf8JsonReader reader)
        {
            var result = new NicknameData();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
                 if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();

                    if (propertyName == "name")
                    {
                        result.name = reader.GetString();
                    }
                    else if (propertyName == "brname")
                    {
                        result.brname = reader.GetString();
                    }
                    else if (propertyName == "brNickName")
                    {
                        result.brNickName = reader.GetString();
                    }
                }
            }
            return result;
        }
    }
}