﻿using AbstractCarRepairShopContracts.BindingModels;
using AbstractCarRepairShopContracts.StoragesContracts;
using AbstractCarRepairShopContracts.ViewModels;
using AbstractCarRepairShopDatabaseImplement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractCarRepairShopDatabaseImplement.Implements
{
    public class OrderStorage : IOrderStorage
    {
        public List<OrderViewModel> GetFullList()
        {
            using var context = new AbstractCarRepairShopDatabase();
            return context.Orders
            .Select(CreateModel)
            .ToList();
        }
        public List<OrderViewModel> GetFilteredList(OrderBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            if (model.DateFrom != null && model.DateTo != null) { 
            using var context = new AbstractCarRepairShopDatabase();
            return context.Orders
            .Where(rec => rec.DateCreate >= model.DateFrom && rec.DateCreate <= model.DateTo)
            //.Where(rec => rec.RepairId == model.RepairId 
            //   && rec.DateCreate >= model.DateFrom && rec.DateCreate <= model.DateTo)
            .Select(CreateModel)
            .ToList();
            }
            using var context_ = new AbstractCarRepairShopDatabase();
            return context_.Orders
            .Where(rec => rec.RepairId == model.RepairId)
            .Select(CreateModel)
            .ToList();


    }
        public OrderViewModel GetElement(OrderBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new AbstractCarRepairShopDatabase();
            var order = context.Orders
            .FirstOrDefault(rec => rec.Id == model.Id);
            return order != null ? CreateModel(order) : null;
        }
        public void Insert(OrderBindingModel model)
        {
            using (var context = new AbstractCarRepairShopDatabase())
            {
                Order order = new Order
                {
                    RepairId = model.RepairId,
                    Count = model.Count,
                    Sum = model.Sum,
                    Status = model.Status,
                    DateCreate = model.DateCreate,
                    DateImplement = model.DateImplement,
                };
                context.Orders.Add(order);
                context.SaveChanges();
                CreateModel(model, order);
                context.SaveChanges();
            }
        }
        public void Update(OrderBindingModel model)
        {
            using (var context = new AbstractCarRepairShopDatabase())
            {
                var element = context.Orders.FirstOrDefault(rec => rec.Id == model.Id);
                if (element == null)
                {
                    throw new Exception("Элемент не найден");
                }
                element.RepairId = model.RepairId;
                element.Count = model.Count;
                element.Sum = model.Sum;
                element.Status = model.Status;
                element.DateCreate = model.DateCreate;
                element.DateImplement = model.DateImplement;
                CreateModel(model, element);
                context.SaveChanges();
            }
        }
        public void Delete(OrderBindingModel model)
        {
            using (var context = new AbstractCarRepairShopDatabase())
            {
                Order element = context.Orders.FirstOrDefault(rec => rec.Id == model.Id);
                if (element != null)
                {
                    context.Orders.Remove(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
        }
        private Order CreateModel(OrderBindingModel model, Order order)
        {
            if (model == null)
            {
                return null;
            }

            using (var context = new AbstractCarRepairShopDatabase())
            {
                Repair element = context.Repairs.FirstOrDefault(rec => rec.Id == model.RepairId);
                if (element != null)
                {
                    if (element.Orders == null)
                    {
                        element.Orders = new List<Order>();
                    }
                    element.Orders.Add(order);
                    context.Repairs.Update(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
            return order;
        }
        private static OrderViewModel CreateModel(Order order)
        {
            using var context = new AbstractCarRepairShopDatabase();
            return new OrderViewModel
            {
                Id = order.Id,
                RepairId = order.RepairId,
                RepairName = context.Repairs.FirstOrDefault(pr => pr.Id == order.RepairId).RepairName, 
                Count = order.Count,
                Sum = order.Sum,
                Status = order.Status,
                DateCreate = order.DateCreate,
                DateImplement = order.DateImplement,
            };
        }
    }
}
