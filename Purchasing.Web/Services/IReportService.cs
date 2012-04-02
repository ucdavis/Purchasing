using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Purchasing.Core.Domain;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Purchasing.Web.Services
{
    public interface IReportService
    {
        byte[] GetInvoice(Order order);
    }

    public class ReportService : IReportService
    {
        // base color
        private CMYKColor _baseColor = new CMYKColor(0.9922f, 0.4264f, 0.0000f, 0.4941f);

        // standard body font
        private readonly Font _font = new Font(Font.FontFamily.TIMES_ROMAN, 10);
        private readonly Font _tableHeaderFont = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.NORMAL, BaseColor.WHITE);
        private readonly Font _boldFont = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD);
        private readonly Font _italicFont = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.ITALIC);
        private readonly Font _headerFont = new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD, new CMYKColor(0.9922f, 0.4264f, 0.0000f, 0.4941f));
        private readonly Font _subHeaderFont = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD, new CMYKColor(0.9922f, 0.4264f, 0.0000f, 0.4941f));
        private readonly Font _sectionHeaderFont = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, new CMYKColor(0.9922f, 0.4264f, 0.0000f, 0.4941f));
        private readonly Font _captionFont = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.NORMAL, new CMYKColor(0.9922f, 0.4264f, 0.0000f, 0.4941f));

        // width of the content
        private float _pageWidth;
        private float _pageHeight;

        private Document InitializeDocument()
        {
            //var doc = new iTextSharp.text.Document(PageSize.LETTER, 36 /* left */, 36 /* right */, 62 /* top */, 52 /* bottom */);

            var doc = new iTextSharp.text.Document(PageSize.LETTER, 36 /* left */, 36 /* right */, 40 /* top */, 52 /* bottom */);

            // set the variable for the page's actual content size
            _pageWidth = doc.PageSize.Width - (doc.LeftMargin + doc.RightMargin);
            _pageHeight = doc.PageSize.Height - (doc.TopMargin + doc.BottomMargin);

            return doc;
        }

        public byte[] GetInvoice(Order order)
        {
            var doc = InitializeDocument();
            var ms = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc, ms);

            doc.Open();

            AddHeader(doc);

            AddTopSection(doc, order);

            AddLineItems(doc, order);

            AddBottomSection(doc, order);

            doc.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// Adds header image and any branding
        /// </summary>
        /// <param name="doc"></param>
        private void AddHeader(Document doc)
        {
            var logo = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Images/PrePurchasing-Logo.png"));
            var table = InitializeTable();
            var cell = InitializeCell();

            // resize the logo
            logo.ScalePercent(20);

            // style the cell
            cell.BackgroundColor = _baseColor;
            
            cell.AddElement(logo);
            table.AddCell(cell);
            doc.Add(table);
        }
                             
        /// <summary>
        /// Adds information before line items
        /// </summary>
        /// <param name="doc"></param>                                                                                  
        /// <param name="order"></param>
        private void AddTopSection(Document doc, Order order)
        {
            // put up the header
            var table = InitializeTable(2);
            
            // cell for order request number
            var ornCell = InitializeCell();
            ornCell.AddElement(new Phrase(string.Format("Order Request #: {0}", order.OrderRequestNumber()), _headerFont));
            ornCell.AddElement(new Phrase(string.Format("Request Placed : {0}", order.DateCreated), _font));
            table.AddCell(ornCell);

            // cell for order status
            var sCell = InitializeCell(new Phrase(string.Format("Status: {0}", order.StatusCode.Name), _font), false, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE);
            table.AddCell(sCell);
            
            // cell for vendor
            var vCell = InitializeCell();
            vCell.AddElement(new Phrase("Vendor:", _font));
            vCell.AddElement(new Phrase(order.Vendor.Name, _font));
            vCell.AddElement(new Phrase(order.Vendor.Line1, _font));
            vCell.AddElement(new Phrase(order.Vendor.Line2, _font));
            vCell.AddElement(new Phrase(order.Vendor.Line3, _font));
            vCell.AddElement(new Phrase(string.Format("{0}, {1} {2}",order.Vendor.City, order.Vendor.State, order.Vendor.Zip), _font));
            vCell.AddElement(new Phrase(string.Format("Phone #: {0}", order.Vendor.Phone), _font));
            table.AddCell(vCell);

            // cell for recipient
            var rCell = InitializeCell();
            rCell.AddElement(new Phrase("Recipient:", _font));
            rCell.AddElement(new Phrase(string.Format("{0} ({1})", order.DeliverTo, order.DeliverToEmail), _font));
            rCell.AddElement(new Phrase(new Phrase(string.Format("{0}, {1}", order.Address.Address, order.Address.Building), _font)));
            rCell.AddElement(new Phrase(new Phrase(string.Format("{0}, {1} {2}", order.Address.City, order.Address.State, order.Address.Zip), _font)));
            table.AddCell(rCell);

            // cell for justification
            var jCell = InitializeCell();
            jCell.Colspan = 2;
            jCell.AddElement(new Phrase("Justification:", _font));
            jCell.AddElement(new Phrase(order.Justification, _font));
            table.AddCell(jCell);

            doc.Add(table);            
        }

        private void AddLineItems(Document doc, Order order)
        {
            var table = InitializeTable(5);
            table.SetWidths(new float[] {1f, 1f, 1f, 4f, 1f});

            // add table headers
            table.AddCell(InitializeCell(new Phrase("Qty.", _tableHeaderFont), true));
            table.AddCell(InitializeCell(new Phrase("Unit", _tableHeaderFont), true));
            table.AddCell(InitializeCell(new Phrase("Catalog #", _tableHeaderFont), true));
            table.AddCell(InitializeCell(new Phrase("Description", _tableHeaderFont), true));
            table.AddCell(InitializeCell(new Phrase("Unit $", _tableHeaderFont), true));

            // line item
            foreach (var li in order.LineItems)
            {
                table.AddCell(InitializeCell(new Phrase(li.Quantity.ToString(), _font)));
                table.AddCell(InitializeCell(new Phrase(li.Unit, _font)));
                table.AddCell(InitializeCell(new Phrase(li.CatalogNumber, _font)));
                table.AddCell(InitializeCell(new Phrase(li.Description, _font)));
                table.AddCell(InitializeCell(new Phrase(li.UnitPrice.ToString("c"), _font), halignment:Element.ALIGN_RIGHT));
            }

            // foot of table
            table.AddCell(InitializeCell(new Phrase("Subtotal:", _font), halignment:Element.ALIGN_RIGHT, colspan:4));
            table.AddCell(InitializeCell(new Phrase(order.Total().ToString("c"), _font), halignment:Element.ALIGN_RIGHT));

            table.AddCell(InitializeCell(new Phrase("Shipping:", _font), halignment: Element.ALIGN_RIGHT, colspan: 4));
            table.AddCell(InitializeCell(new Phrase(order.ShippingAmount.ToString("c"), _font), halignment: Element.ALIGN_RIGHT));

            table.AddCell(InitializeCell(new Phrase("Handling:", _font), halignment: Element.ALIGN_RIGHT, colspan: 4));
            table.AddCell(InitializeCell(new Phrase(order.FreightAmount.ToString("c"), _font), halignment: Element.ALIGN_RIGHT));

            table.AddCell(InitializeCell(new Phrase("Tax:", _font), halignment: Element.ALIGN_RIGHT, colspan: 4));
            table.AddCell(InitializeCell(new Phrase(order.EstimatedTax.ToString("c"), _font), halignment: Element.ALIGN_RIGHT));
            
            table.AddCell(InitializeCell(new Phrase("Total:", _font), halignment: Element.ALIGN_RIGHT, colspan: 4));
            table.AddCell(InitializeCell(new Phrase(order.GrandTotal().ToString("c"), _font), halignment: Element.ALIGN_RIGHT));

            doc.Add(table);
        }

        private void AddBottomSection(Document doc, Order order)
        {
            var table = InitializeTable(4);

            // add header for the table
            var hcell1 = InitializeCell(new Phrase("Date", _tableHeaderFont), true);
            var hcell2 = InitializeCell(new Phrase("Action", _tableHeaderFont), true);
            var hcell3 = InitializeCell(new Phrase("Role", _tableHeaderFont), true);
            var hcell4 = InitializeCell(new Phrase("User", _tableHeaderFont), true);

            table.AddCell(hcell1);
            table.AddCell(hcell2); 
            table.AddCell(hcell3); 
            table.AddCell(hcell4);

            foreach (var tracking in order.OrderTrackings.OrderBy(x=>x.DateCreated))
            {
                var cell1 = InitializeCell(new Phrase(tracking.DateCreated.ToString(), _font));
                var cell2 = InitializeCell(new Phrase(tracking.Description, _font));
                var cell3 = InitializeCell(new Phrase(tracking.StatusCode.Name, _font));
                var cell4 = InitializeCell(new Phrase(tracking.User.FullName, _font));

                table.AddCell(cell1);
                table.AddCell(cell2);
                table.AddCell(cell3);
                table.AddCell(cell4);
            }

            doc.Add(table);
        }

        private PdfPTable InitializeTable(int columns = 1)
        {
            var table = new PdfPTable(columns);

            // set the styles
            table.TotalWidth = _pageWidth;
            table.LockedWidth = true;
            table.SpacingAfter = 2f;

            return table;
        }
        private PdfPCell InitializeCell(Phrase phrase = null, bool header = false, int? halignment = null, int? valignment = null, int? colspan = null)
        {
            var cell = new PdfPCell();
            if (phrase!= null) cell = new PdfPCell(phrase);

            if (header)
            {
                cell.BackgroundColor = _baseColor;
            }

            if (halignment.HasValue)
            {
                cell.HorizontalAlignment = halignment.Value;
            }

            if (valignment.HasValue)
            {
                cell.VerticalAlignment = valignment.Value;
            }

            if (colspan.HasValue)
            {
                cell.Colspan = colspan.Value;
            }

            cell.Padding = 10;

            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;

            return cell;
        }
    }
}
