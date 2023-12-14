using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommonLibrary.Utilities
{
    public static class Utility
    {
        public static byte[] SerializeAndGetBytes(object obj) => obj is not null ? Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj)) : default;

        public static string JsonObjectSerializer(object obj) => obj is not null ? JsonSerializer.Serialize(obj) : default;
    }
}
