using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        // colors
        private readonly BaseColor _headerColor = BaseColor.GRAY;
        private readonly BaseColor _tableDataColor = BaseColor.LIGHT_GRAY;
        private readonly BaseColor _subTableHeaderColor = BaseColor.LIGHT_GRAY;

        // standard body font
        private readonly Font _pageHeaderFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
        private readonly Font _headerFont = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD);
        
        private readonly Font _font = new Font(Font.FontFamily.TIMES_ROMAN, 10);
        private readonly Font _boldFont = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD);
        private readonly Font _italicFont = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.ITALIC);

        private readonly Font _tableHeaderFont = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD, BaseColor.WHITE);
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

            AddComments(doc, order);

            doc.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// Adds header image and any branding
        /// </summary>
        /// <param name="doc"></param>
        private void AddHeader(Document doc)
        {
            var table = InitializeTable(2);
            var cell = InitializeCell("UCD PrePurchasing - Order Request", _pageHeaderFont, valignment: Element.ALIGN_MIDDLE);
            var cell2 = InitializeCell("-- Internal Use Only --", _pageHeaderFont, valignment: Element.ALIGN_MIDDLE, halignment: Element.ALIGN_RIGHT);

            // style the cell
            cell.BackgroundColor = _headerColor;
            cell.BorderColor = _headerColor;
            cell2.BackgroundColor = _headerColor;
            cell2.BorderColor = _headerColor;

            table.AddCell(cell);
            table.AddCell(cell2);
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
            //var ornCell = InitializeCell(string.Format("Request #: {0}", order.OrderRequestNumber()), _headerFont, valignment:Element.ALIGN_MIDDLE);
            //if (!string.IsNullOrEmpty(order.ReferenceNumber))
            //{
            //    ornCell.AddElement(new Phrase(string.Format("Reference #: {0}", order.ReferenceNumber), _font));    
            //}

            var ornCell = InitializeCell(halignment:Element.ALIGN_LEFT);
            var ornTable = new PdfPTable(1);
            ornTable.AddCell(InitializeCell(string.Format("Request #: {0}", order.OrderRequestNumber()), _headerFont, valignment: Element.ALIGN_MIDDLE, halignment:Element.ALIGN_LEFT, bottomBorder:false));
            ornTable.AddCell(InitializeCell(string.Format("Reference #: {0}", order.ReferenceNumber), _font, valignment: Element.ALIGN_MIDDLE, halignment: Element.ALIGN_LEFT, bottomBorder: false));
            ornCell.AddElement(ornTable);

            table.AddCell(ornCell);
                      
            // order status cell
            var sCell = InitializeCell();
            var stable = new PdfPTable(2);
            stable.AddCell(InitializeCell("Request Placed:", _boldFont, padding: false, halignment: Element.ALIGN_RIGHT, bottomBorder: false));
            stable.AddCell(InitializeCell(order.DateCreated.ToString(), padding: false, bottomBorder: false));
            stable.AddCell(InitializeCell("Status: ", _boldFont, padding: false, halignment: Element.ALIGN_RIGHT, bottomBorder: false));
            stable.AddCell(InitializeCell(order.StatusCode.Name, padding: false, bottomBorder: false));
            stable.AddCell(InitializeCell("Date Needed: ", _boldFont, padding: false, halignment: Element.ALIGN_RIGHT, bottomBorder: false));
            stable.AddCell(InitializeCell(order.DateNeeded.ToString("d"), padding: false, bottomBorder: false));
            sCell.AddElement(stable);
            table.AddCell(sCell);

            var aCell = InitializeCell(colspan:2);
            
            var atable = InitializeTable(4);
            atable.SetWidths(new int[4]{50,150,50,150});
            
            var acell1 = InitializeCell("Vendor:", _boldFont, bottomBorder:false);
            var acell2 = InitializeCell(topBottomBorders: false, bottomBorder: false);
            acell2.AddElement(new Phrase(order.Vendor == null ? "-- Unspecified --" : order.Vendor.Name, _font));
            acell2.AddElement(new Phrase(order.Vendor == null ? string.Empty : order.Vendor.Line1, _font));
            acell2.AddElement(new Phrase(order.Vendor == null ? string.Empty : order.Vendor.Line2, _font));
            acell2.AddElement(new Phrase(order.Vendor == null ? string.Empty : order.Vendor.Line3, _font));
            acell2.AddElement(new Phrase(order.Vendor == null ? string.Empty : string.Format("{0}, {1} {2}", order.Vendor.City, order.Vendor.State, order.Vendor.Zip), _font));
            acell2.AddElement(new Phrase(order.Vendor == null ? string.Empty : string.Format("Phone #: {0}", order.Vendor.Phone), _font));
            var acell3 = InitializeCell("Recipient:", _boldFont, bottomBorder: false);
            var acell4 = InitializeCell(bottomBorder: false);
            acell4.AddElement(new Phrase(string.Format("{0} ({1})", order.DeliverTo, order.DeliverToEmail), _font));
            acell4.AddElement(new Phrase(new Phrase(string.Format("{0}, {1}", order.Address.Address, order.Address.Building), _font)));
            acell4.AddElement(new Phrase(new Phrase(string.Format("{0}, {1} {2}", order.Address.City, order.Address.State, order.Address.Zip), _font)));
            if (!string.IsNullOrEmpty(order.DeliverToPhone) || !string.IsNullOrEmpty(order.Address.Phone))
            {
                acell4.AddElement(new Phrase(string.Format("Ph: {0}", !string.IsNullOrEmpty(order.DeliverToPhone) ? order.DeliverToPhone : order.Address.Phone), _font));    
            }

            atable.AddCell(acell1);
            atable.AddCell(acell2); 
            atable.AddCell(acell3);
            atable.AddCell(acell4);
            aCell.AddElement(atable);
            table.AddCell(aCell);
            
            // cell for justification
            var jCell = InitializeCell(colspan:2, bottomBorder:false);
            jCell.Padding = 10;
            jCell.AddElement(new Phrase("Justification:", _boldFont));
            jCell.AddElement(new Phrase(order.Justification, _font));
            table.AddCell(jCell);

            doc.Add(table);            
        }
        
        private void AddLineItems(Document doc, Order order)
        {
            var table = InitializeTable(6);
            table.SetWidths(new float[] {1f, 1f, 1f, 4f, 1.5f, 1.5f});

            // add table headers
            table.AddCell(InitializeCell("Qty.", _tableHeaderFont, true));
            table.AddCell(InitializeCell("Unit", _tableHeaderFont, true));
            table.AddCell(InitializeCell("Catalog #", _tableHeaderFont, true));
            table.AddCell(InitializeCell("Description", _tableHeaderFont, true));
            table.AddCell(InitializeCell("Unit $", _tableHeaderFont, true));
            table.AddCell(InitializeCell("Line $", _tableHeaderFont, true));

            // line item
            ProcessLineItems(table, order);

            // foot of table
            table.AddCell(InitializeCell("Subtotal:", _boldFont, halignment: Element.ALIGN_RIGHT, colspan: 5, bottomBorder: false));
            table.AddCell(InitializeCell(order.Total().ToString("c"), _font, halignment: Element.ALIGN_RIGHT, bottomBorder: false));

            table.AddCell(InitializeCell("Shipping:", _boldFont, halignment: Element.ALIGN_RIGHT, colspan: 5, bottomBorder: false));
            table.AddCell(InitializeCell(order.ShippingAmount.ToString("c"), _font, halignment: Element.ALIGN_RIGHT, bottomBorder: false));

            table.AddCell(InitializeCell("Handling:", _boldFont, halignment: Element.ALIGN_RIGHT, colspan: 5, bottomBorder: false));
            table.AddCell(InitializeCell(order.FreightAmount.ToString("c"), _font, halignment: Element.ALIGN_RIGHT, bottomBorder: false));

            table.AddCell(InitializeCell(string.Format("Tax: ({0}%)", order.EstimatedTax), _boldFont, halignment: Element.ALIGN_RIGHT, colspan: 5, bottomBorder: false));
            table.AddCell(InitializeCell(order.Tax().ToString("c"), _font, halignment: Element.ALIGN_RIGHT, bottomBorder: false));

            table.AddCell(InitializeCell("Total:", _boldFont, halignment: Element.ALIGN_RIGHT, colspan: 5));
            table.AddCell(InitializeCell(order.GrandTotal().ToString("c"), _font, halignment: Element.ALIGN_RIGHT));

            doc.Add(table);
        }

        private void ProcessLineItems(PdfPTable table, Order order)
        {
            foreach (var li in order.LineItems)
            {
                table.AddCell(InitializeCell(li.Quantity.ToString(), _font));
                table.AddCell(InitializeCell(li.Unit, _font));
                table.AddCell(InitializeCell(li.CatalogNumber, _font));
                table.AddCell(InitializeCell(li.Description, _font));
                table.AddCell(InitializeCell(li.UnitPrice.ToString("c"), _font));
                table.AddCell(InitializeCell((li.Quantity * li.UnitPrice).ToString("c"), _font, halignment: Element.ALIGN_RIGHT));

                if (!string.IsNullOrEmpty(li.Url))
                {
                    var urlCell1 = InitializeCell("Url:", _boldFont, colspan: 2, backgroundColor:_tableDataColor, bottomBorder: false);
                    table.AddCell(urlCell1);

                    var urlCell2 = InitializeCell(li.Url, _font, colspan:4, backgroundColor:_tableDataColor, bottomBorder: false);
                    table.AddCell(urlCell2);
                }

                if (li.Commodity != null)
                {
                    var commodityCell1 = InitializeCell("Commodity Code:", _boldFont, colspan: 2, backgroundColor: _tableDataColor, bottomBorder: false);
                    table.AddCell(commodityCell1);

                    var commodityCell2 = InitializeCell(string.Format("{0} ({1})", li.Commodity.Name, li.Commodity.Id), _font, colspan: 4, backgroundColor: _tableDataColor, bottomBorder: false);
                    table.AddCell(commodityCell2);
                }

                if (!string.IsNullOrEmpty(li.Notes))
                {
                    var noteCell1 = InitializeCell("Notes:", _boldFont, colspan: 2, backgroundColor: _tableDataColor, bottomBorder: false);
                    table.AddCell(noteCell1);

                    var noteCell2 = InitializeCell(li.Notes, _font, colspan: 4, backgroundColor: _tableDataColor, bottomBorder: false);
                    table.AddCell(noteCell2);
                }

                if (order.HasLineSplits)
                {
                    var accountTable = SplitAccountingInformation(li.Splits);
                    var acell = InitializeCell(colspan:6);
                    acell.AddElement(accountTable);
                    table.AddCell(acell);
                }
            }
        }

        private PdfPTable SplitAccountingInformation(IEnumerable<Split> splits)
        {
            var table = InitializeTable(4);
            table.TotalWidth = _pageWidth*.75f;

            table.AddCell(InitializeCell("Account", _tableHeaderFont, backgroundColor:_subTableHeaderColor, bottomBorder:false));
            table.AddCell(InitializeCell("Project", _tableHeaderFont, backgroundColor: _subTableHeaderColor, bottomBorder: false));
            table.AddCell(InitializeCell("Amount", _tableHeaderFont, backgroundColor: _subTableHeaderColor, bottomBorder: false));
            table.AddCell(InitializeCell("Distribution", _tableHeaderFont, backgroundColor: _subTableHeaderColor, bottomBorder: false));

            foreach (var split in splits)
            {
                table.AddCell(InitializeCell(split.FullAccountDisplay, _font, bottomBorder: false));
                table.AddCell(InitializeCell(split.Project, _font, bottomBorder:false));
                table.AddCell(InitializeCell(split.Amount.ToString("c"),_font, bottomBorder:false));

                if (split.LineItem == null)
                {
                     table.AddCell(InitializeCell((split.Amount/split.Order.GrandTotalFromDb).ToString("p"), _font, bottomBorder:false));
                }
                else
                {
                    table.AddCell(InitializeCell(((split.Amount / split.LineItem.TotalWithTax())).ToString("p"), _font, bottomBorder:false));    
                }
            }

            return table;
        }

        private void AddBottomSection(Document doc, Order order)
        {
            // order level splits
            if (order.Splits.Count > 1 && !order.HasLineSplits)
            {
                var accountingTable = SplitAccountingInformation(order.Splits);
                doc.Add(accountingTable);
            }
            // just one account
            else if (!order.HasLineSplits)
            {
                var split = order.Splits.First();

                var accountingTable = InitializeTable(2);

                accountingTable.SetWidths(new float[] { 1f, 4f });

                accountingTable.AddCell(InitializeCell("Account:", _boldFont, bottomBorder:false));
                accountingTable.AddCell(InitializeCell(split.FullAccountDisplay, _font, bottomBorder:false));
                accountingTable.AddCell(InitializeCell("Project:", _boldFont));
                accountingTable.AddCell(InitializeCell(split.Project, _font));

                doc.Add(accountingTable);
            }
        }

        private void AddComments(Document doc, Order order)
        {
            if (order.OrderComments.Any())
            {
                var cTable = InitializeTable(3);
                cTable.SetWidths(new float[]{1.5f, 1.75f, 4f});
                cTable.SpacingBefore = 1f;

                cTable.AddCell(InitializeCell("Comments", _tableHeaderFont, true, Element.ALIGN_LEFT, colspan:3, bottomBorder:false));

                foreach (var c in order.OrderComments)
                {
                    cTable.AddCell(InitializeCell(c.DateCreated.ToString(), _font, bottomBorder:false));
                    cTable.AddCell(InitializeCell(c.User.FullName, _font, bottomBorder:false));
                    cTable.AddCell(InitializeCell(c.Text, _font, bottomBorder:false));
                }

                doc.Add(cTable);
            }
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

        private PdfPCell InitializeCell(string text = null, Font font = null, bool header = false, int? halignment = null, int? valignment = null, int? colspan = null, bool sideBorders = false, bool topBottomBorders = false, bool bottomBorder = true, bool padding = true, BaseColor backgroundColor = null)
        {
            var cell = new PdfPCell();
            if (!string.IsNullOrEmpty(text)) cell = new PdfPCell(new Phrase(text, font ?? _font));

            if (header)
            {
                cell.BackgroundColor = _headerColor;
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

            if (backgroundColor != null)
            {
                cell.BackgroundColor = backgroundColor;
            }

            if (padding)
            {
                cell.Padding = 5;
            }

            if (!sideBorders)
            {
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 0;    
            }

            if (!topBottomBorders)
            {
                cell.BorderWidthTop = 0;
                if (!bottomBorder)
                {
                    cell.BorderWidthBottom = 0;    
                }
            }

            return cell;
        }
    }
}
