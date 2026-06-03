using MongoDB.Bson;

namespace UserService.Models
{
    public class User
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public ObjectId Id { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("name")]
        public string? Name { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("password")]
        public string? Password { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("email")]
        public string? Email { get; set; }
    }

    public class Order
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public ObjectId Id { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("name")]
        public string? Name { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("history")]
        public List<object> History { get; set; } = new();
    }

    public class UniqueIdResponse
    {
        public string? Uuid { get; set; }
    }

    public class HealthStatus
    {
        public string? App { get; set; }
        public bool Mongo { get; set; }
    }

    public class LoginRequest
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
    }

    public class RegisterRequest
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
}
