﻿using drug_store_api.data;
using drug_store_api.entities.SalesOrders;
using drug_store_api.repositories.IF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.repositories.Factory
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly DrugStoreDbContext _context;
        public SalesOrderRepository(DrugStoreDbContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<SalesOrder>> GetSalesOrderByCreatedDate(DateTime createdDate)
        {
            return await _context.SalesOrders.Where(x => x.CreatedAt == createdDate).ToListAsync();
        }

        public Task<SalesOrder> GetSalesOrderDetails()
        {
            throw new NotImplementedException();
        }
    }
}
