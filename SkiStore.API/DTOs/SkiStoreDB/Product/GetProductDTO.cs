﻿namespace SkiStore.API.DTOs.SkiStoreDB.Product
{
    public class GetProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }


        public string Description { get; set; }

        public float Price { get; set; }

        public string PictureUrl { get; set; }

        public string Type { get; set; }

        public string Brand { get; set; }

        public int QuantityInStock { get; set; }
    }
}
