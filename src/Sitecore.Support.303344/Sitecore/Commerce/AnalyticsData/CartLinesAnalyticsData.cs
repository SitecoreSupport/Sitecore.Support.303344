
namespace Sitecore.Support.Commerce.AnalyticsData
{
  using Sitecore.Commerce.AnalyticsData;
  using Sitecore.Commerce.Engine.Connect.Entities;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Services.Carts;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class CartLinesAnalyticsData : Sitecore.Commerce.AnalyticsData.CartLinesAnalyticsData
  {
    private bool _isDeleteOperation;
    protected override void InitializeCartLineData(IEnumerable<CartLine> cartLines, Cart cart)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(cartLines, "cartLines");
      List<CartLine> list = cartLines.Where(delegate (CartLine cartline)
      {
        if (cartline != null)
        {
          return cartline.Product != null;
        }
        return false;
      }).ToList();
      if (list.Count > 0 && cart.Lines != null)
      {
        CartLines = new List<CartLineAnalyticsData>();
        foreach (CartLine item in list)
        {
          CartLine cartLine2;
          if (_isDeleteOperation)
          {
            cartLine2 = item;
          }
          else
          {
            cartLine2 = cart.Lines.Where(delegate (CartLine l)
            {
              if (l.Product != null)
              {

                //Bug fix for 303344
                if (l.Product is CommerceCartProduct && item.Product is CommerceCartProduct)
                {
                  return (l.Product.ProductId.Equals(item.Product.ProductId, StringComparison.OrdinalIgnoreCase) &&
                  ((CommerceCartProduct)l.Product).ProductVariantId.Equals(((CommerceCartProduct)item.Product).ProductVariantId));
                }
                //End bug fix
              }
              return false;
            }).FirstOrDefault();
            if (cartLine2 == null)
            {
              cartLine2 = item;
            }
          }
          CartLineAnalyticsData cartLineAnalyticsData = AnalyticsDataInitializerFactory.Create<CartLineAnalyticsData>();
          cartLineAnalyticsData.Initialize(base.ServicePipelineArgs, cartLine2);
          CartLines.Add(cartLineAnalyticsData);
        }
      }
    }

    protected override Cart GetCartFromArgs()
    {
      _isDeleteOperation = (base.ServicePipelineArgs.Request is RemoveCartLinesRequest);
      if (base.ServicePipelineArgs.Result is CartResult)
      {
        return ((CartResult)base.ServicePipelineArgs.Result).Cart;
      }
      return null;
    }
  }
}