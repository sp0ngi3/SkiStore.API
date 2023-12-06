﻿namespace SkiStore.API.Models.SkiStoreDB.Product
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }    


        public string Description { get; set; }

        public float Price { get; set; } 

        public string PictureUrl {  get; set; } 

        public string Type { get; set; }

        public string Brand { get; set; }

        public int QuantityInStockk { get; set; }
    }
}
