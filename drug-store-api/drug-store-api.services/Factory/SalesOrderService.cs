using AutoMapper;
using drug_store_api.dtos.SaleOrders;
using drug_store_api.repositories.IF;
using drug_store_api.services.IF;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.services.Factory
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISalesOrderRepository _repository;
        private readonly IMapper _mapper;
        public SalesOrderService(ISalesOrderRepository repository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<SalesOrderDto>> GetSaleOrderByCreatedDate(DateTime createdDate)
        {
            var saleOrder = await _repository.GetSalesOrderByCreatedDate(createdDate);
            var res = _mapper.Map<List<SalesOrderDto>>(saleOrder);
            return res;
        }
    }
}
