﻿namespace CryptoTaxV3.Domain.Products
{
    public class ProductDto
    {
        public int Id { get; init; }
        public TxSource Source { get; init; }
        public string Name { get; init; }
    }
}
