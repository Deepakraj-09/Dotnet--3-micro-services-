using MongoDB.Bson;

namespace CatalogueService.Models
{
    public class Product
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public ObjectId Id { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("sku")]
        public string? Sku { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("name")]
        public string? Name { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("description")]
        public string? Description { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("price")]
        public double Price { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("instock")]
        public int Instock { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("categories")]
        public List<string>? Categories { get; set; }
    }

    public class HealthStatus
    {
        public string? App { get; set; }
        public bool Mongo { get; set; }
    }
}
