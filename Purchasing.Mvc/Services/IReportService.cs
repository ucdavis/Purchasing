using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText.Layout;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Colors;
using iText.IO.Font.Constants;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Borders;
using iText.Layout.Properties;
using Purchasing.Core.Domain;

namespace Purchasing.Mvc.Services
{
    public interface IReportService
    {
        byte[] GetInvoice(Order order, bool showOrderHistory = false, bool forVendor = false);
    }

    public class ReportService : IReportService
    {
        // colors
        private readonly Color _headerColor = ColorConstants.GRAY;
        private readonly Color _tableDataColor = ColorConstants.LIGHT_GRAY;
        private readonly Color _subTableHeaderColor = ColorConstants.LIGHT_GRAY;

        // standard body font
        private readonly PdfFont _pageHeaderFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA); //12, Font.BOLD, ColorConstants.WHITE);
        private readonly PdfFont _headerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA); //, 14, Font.BOLD);

        private readonly PdfFont _font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN); //, 10);
        private readonly PdfFont _boldFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN); //, 10, Font.BOLD);
        private readonly PdfFont _italicFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN); //, 10, Font.ITALIC);
        private readonly PdfFont _smallPrint = PdfFontFactory.CreateFont(StandardFonts.HELVETICA); //, 8);

        private readonly PdfFont _tableHeaderFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN); //, 10, Font.BOLD, ColorConstants.WHITE);
        private readonly PdfFont _subHeaderFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA); //, 14, Font.BOLD, new CMYKColor(0.9922f, 0.4264f, 0.0000f, 0.4941f));
        private readonly PdfFont _sectionHeaderFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA); //, 10, Font.BOLD, new CMYKColor(0.9922f, 0.4264f, 0.0000f, 0.4941f));
        private readonly PdfFont _captionFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN); //, 10, Font.NORMAL, new CMYKColor(0.9922f, 0.4264f, 0.0000f, 0.4941f));

        // width of the content
        private float _pageWidth;
        private float _pageHeight;

        private Document InitializeDocument(PdfWriter writer)
        {
            var pdf = new PdfDocument(writer);

            var doc = new Document(pdf, PageSize.LETTER);
            doc.SetTopMargin(40);
            doc.SetBottomMargin(52);
            doc.SetLeftMargin(36);
            doc.SetRightMargin(36);

            // set the variable for the page's actual content size
            _pageWidth = PageSize.LETTER.GetWidth() - (doc.GetLeftMargin() + doc.GetRightMargin());
            _pageHeight = PageSize.LETTER.GetHeight() - (doc.GetTopMargin() + doc.GetBottomMargin());

            return doc;
        }

        public byte[] GetInvoice(Order order, bool showOrderHistory = false, bool forVendor = false)
        {
            var ms = new MemoryStream();
            var writer = new PdfWriter(ms);

            var doc = InitializeDocument(writer);

            AddHeader(doc, order, forVendor);

            AddTopSection(doc, order, forVendor);

            AddLineItems(doc, order, forVendor);


            if (!forVendor)
            {
                AddBottomSection(doc, order);
                AddControlledSubstanceSection(doc, order);
                AddComments(doc, order);
            }

            if (showOrderHistory)
            {
                AddOrderHistory(doc, order);
            }

            if (forVendor)
            {
                AddSpecialFooter(doc, order);
            }


            doc.Close();
            return ms.ToArray();
        }

        private void AddSpecialFooter(Document doc, Order order)
        {
            var table = InitializeTable(1);
            table.AddCell(InitializeCell(string.Format("Order Request: {0} **", order.OrderRequestNumber()), _font, valignment: VerticalAlignment.TOP, bottomBorder: false));
            table.AddCell(InitializeCell("** The order request number is an internal only number, not a PO number.", _boldFont, valignment: VerticalAlignment.TOP, bottomBorder: false));
            table.AddCell(InitializeCell("** Not a confirmation of order and this is not a University PO.", _boldFont, valignment: VerticalAlignment.TOP));
            doc.Add(table);

        }

        /// <summary>
        /// Adds header image and any branding
        /// </summary>
        /// <param name="doc"></param>
        private void AddHeader(Document doc, Order order, bool forVendor)
        {
            var table = InitializeTable(2);
            var cell = InitializeCell(
                forVendor ? "UCD PrePurchasing" : string.Format("UCD PrePurchasing - Order Request: {0}", order.OrderRequestNumber()), 
                _pageHeaderFont, 
                valignment: VerticalAlignment.MIDDLE,
                width: 70);
            var cell2 = InitializeCell(
                forVendor ? string.Empty : "-- Internal Use Only --", _pageHeaderFont,
                valignment: VerticalAlignment.MIDDLE,
                halignment: HorizontalAlignment.RIGHT,
                width: 30);

            // style the cell
            cell.SetBackgroundColor(_headerColor);
            cell.SetBorder(new SolidBorder(_headerColor, 1));
            cell2.SetBackgroundColor(_headerColor);
            cell2.SetBorder(new SolidBorder(_headerColor, 1));

            table.AddCell(cell);
            table.AddCell(cell2);
            doc.Add(table);
        }

        /// <summary>
        /// Adds information before line items
        /// </summary>
        /// <param name="doc"></param>                                                                                  
        /// <param name="order"></param>
        /// <param name="forVendor">If true, does not print some stuff </param>
        private void AddTopSection(Document doc, Order order, bool forVendor)
        {
            var topTable = InitializeTable(4);

            topTable.AddCell(InitializeCell("Reference #:", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 15));
            topTable.AddCell(InitializeCell(!string.IsNullOrWhiteSpace(order.ReferenceNumber) ? order.ReferenceNumber : "--", _font, halignment: HorizontalAlignment.LEFT, padding: false, bottomBorder: false, width: 57));
            topTable.AddCell(InitializeCell("Request Placed:", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 15));
            topTable.AddCell(InitializeCell(order.DateCreated.ToString(), padding: false, bottomBorder: false, width: 23));

            topTable.AddCell(InitializeCell("Workgroup:", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 15));
            topTable.AddCell(InitializeCell(order.Workgroup.Name, _font, halignment: HorizontalAlignment.LEFT, padding: false, bottomBorder: false, width: 57));
            topTable.AddCell(InitializeCell("Status: ", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 15));
            topTable.AddCell(InitializeCell(order.StatusCode.Name, padding: false, bottomBorder: false, width: 23));

            topTable.AddCell(InitializeCell("Department:", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 15));
            topTable.AddCell(InitializeCell(order.Organization.Name, _font, halignment: HorizontalAlignment.LEFT, padding: false, bottomBorder: false, width: 57));
            topTable.AddCell(InitializeCell("Date Needed: ", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 15));
            topTable.AddCell(InitializeCell(order.DateNeeded.ToString("d"), padding: false, bottomBorder: false, width: 23));

            topTable.AddCell(InitializeCell("", colspan: 4));
            doc.Add(topTable);

            // put up the header
            var table = InitializeTable(2);

            var aCell = InitializeCell(colspan: 2);

            var atable = InitializeTable(4);

            if (forVendor)
            {
                //Nothing
            }
            else
            {
                var acell1 = InitializeCell("Vendor:", _boldFont, bottomBorder: false, width: 50);
                var acell2 = InitializeCell(topBottomBorders: false, bottomBorder: false, width: 150);
                acell2.Add(new Paragraph(order.Vendor == null ? "-- Unspecified --" : order.Vendor.Name).SetFont(_font));
                acell2.Add(new Paragraph(order.Vendor == null ? string.Empty : order.Vendor.Line1).SetFont(_font));
                acell2.Add(new Paragraph(order.Vendor == null ? string.Empty : order.Vendor.Line2).SetFont(_font));
                acell2.Add(new Paragraph(order.Vendor == null ? string.Empty : order.Vendor.Line3).SetFont(_font));
                acell2.Add(new Paragraph(order.Vendor == null ? string.Empty : string.Format("{0}, {1} {2}", order.Vendor.City, order.Vendor.State, order.Vendor.Zip)).SetFont(_font));
                acell2.Add(new Paragraph(order.Vendor == null ? string.Empty : string.Format("Phone #: {0}", order.Vendor.Phone)).SetFont(_font));
                atable.AddCell(acell1);
                atable.AddCell(acell2);
                var acell3 = InitializeCell("Recipient:", _boldFont, bottomBorder: false, width: 50);
                var acell4 = InitializeCell(bottomBorder: false, width: 150);
                acell4.Add(new Paragraph(string.Format("{0} ({1})", order.DeliverTo, order.DeliverToEmail)).SetFont(_font));
                acell4.Add(new Paragraph(string.Format("{0}, {1}", order.Address.Address, order.Address.Building)).SetFont(_font));
                acell4.Add(new Paragraph(string.Format("{0}, {1} {2}", order.Address.City, order.Address.State, order.Address.Zip)).SetFont(_font));
                if (!string.IsNullOrEmpty(order.DeliverToPhone) || !string.IsNullOrEmpty(order.Address.Phone))
                {
                    acell4.Add(new Paragraph(string.Format("Ph: {0}", !string.IsNullOrEmpty(order.DeliverToPhone) ? order.DeliverToPhone : order.Address.Phone)).SetFont(_font));
                }

                atable.AddCell(acell3);
                atable.AddCell(acell4);


                aCell.Add(atable);
                table.AddCell(aCell);

            }


            // cell for justification
            var jCell = InitializeCell(colspan: 2, bottomBorder: false);
            jCell.SetPadding(10);
            jCell.Add(new Paragraph("Justification:").SetFont(_boldFont));
            jCell.Add(new Paragraph(order.Justification).SetFont(_font));
            table.AddCell(jCell);

            if (!string.IsNullOrWhiteSpace(order.BusinessPurpose))
            {
                var bpCell = InitializeCell(colspan: 2, bottomBorder: false);
                bpCell.SetPadding(10);
                bpCell.Add(new Paragraph(string.Format("Business Purpose: {0}", order.RequestType)).SetFont(_boldFont));
                bpCell.Add(new Paragraph(order.BusinessPurpose).SetFont(_font));
                table.AddCell(bpCell);
            }

            doc.Add(table);
        }

        private void AddLineItems(Document doc, Order order, bool forVendor)
        {
            var table = InitializeTable(7);

            // add table headers
            table.AddCell(InitializeCell(string.Empty, _tableHeaderFont, true, width: 0.5f));
            table.AddCell(InitializeCell("Qty.", _tableHeaderFont, true, width: 1f));
            table.AddCell(InitializeCell("Unit", _tableHeaderFont, true, width: 1f));
            table.AddCell(InitializeCell("Catalog #", _tableHeaderFont, true, width: 1f));
            table.AddCell(InitializeCell("Description", _tableHeaderFont, true, width: 4f));
            table.AddCell(InitializeCell("Unit $", _tableHeaderFont, true, width: 1.5f));
            table.AddCell(InitializeCell("Line $", _tableHeaderFont, true, width: 1.5f));

            // line item
            ProcessLineItems(table, order, forVendor);

            // foot of table
            table.AddCell(InitializeCell("Subtotal:", _boldFont, halignment: HorizontalAlignment.RIGHT, colspan: 6, bottomBorder: false));
            table.AddCell(InitializeCell(forVendor ? string.Empty : order.Total().ToString("c"), _font, halignment: HorizontalAlignment.RIGHT, bottomBorder: false));

            table.AddCell(InitializeCell("Estimated Freight:", _boldFont, halignment: HorizontalAlignment.RIGHT, colspan: 6, bottomBorder: false));
            table.AddCell(InitializeCell(forVendor ? string.Empty : order.FreightAmount.ToString("c"), _font, halignment: HorizontalAlignment.RIGHT, bottomBorder: false));

            table.AddCell(InitializeCell("Estimated Shipping and Handling:", _boldFont, halignment: HorizontalAlignment.RIGHT, colspan: 6, bottomBorder: false));
            table.AddCell(InitializeCell(forVendor ? string.Empty : order.ShippingAmount.ToString("c"), _font, halignment: HorizontalAlignment.RIGHT, bottomBorder: false));

            table.AddCell(InitializeCell(string.Format("Estimated Tax: ({0}%)", order.EstimatedTax), _boldFont, halignment: HorizontalAlignment.RIGHT, colspan: 6, bottomBorder: false));
            table.AddCell(InitializeCell(forVendor ? string.Empty : order.Tax().ToString("c"), _font, halignment: HorizontalAlignment.RIGHT, bottomBorder: false));

            table.AddCell(InitializeCell("Total:", _boldFont, halignment: HorizontalAlignment.RIGHT, colspan: 6));
            table.AddCell(InitializeCell(forVendor ? string.Empty : order.GrandTotal().ToString("c"), _font, halignment: HorizontalAlignment.RIGHT));

            doc.Add(table);
        }

        private void ProcessLineItems(Table table, Order order, bool forVendor)
        {
            var count = 1;
            foreach (var li in order.LineItems)
            {
                table.AddCell(InitializeCell(count++.ToString(), _font));
                table.AddCell(InitializeCell(string.Format("{0:0.###}", li.Quantity), _font));
                table.AddCell(InitializeCell(li.Unit, _font));
                table.AddCell(InitializeCell(li.CatalogNumber, _font));
                table.AddCell(InitializeCell(li.Description, _font));
                table.AddCell(InitializeCell(forVendor ? string.Empty : li.UnitPrice.ToString("c"), _font));
                table.AddCell(InitializeCell(forVendor ? string.Empty : (li.Quantity * li.UnitPrice).ToString("c"), _font, halignment: HorizontalAlignment.RIGHT));

                if (!string.IsNullOrEmpty(li.Url))
                {
                    var urlCell1 = InitializeCell("Url:", _boldFont, colspan: 2, backgroundColor: _tableDataColor, bottomBorder: false);
                    table.AddCell(urlCell1);

                    var urlCell2 = InitializeCell(li.Url, _font, colspan: 5, backgroundColor: _tableDataColor, bottomBorder: false);
                    table.AddCell(urlCell2);
                }
                if (!forVendor)
                {
                    if (li.Commodity != null)
                    {
                        var commodityCell1 = InitializeCell("Commodity Code:", _boldFont, colspan: 2, backgroundColor: _tableDataColor, bottomBorder: false);
                        table.AddCell(commodityCell1);

                        var commodityCell2 = InitializeCell(string.Format("{0} ({1})", li.Commodity.Name, li.Commodity.Id), _font, colspan: 5, backgroundColor: _tableDataColor, bottomBorder: false);
                        table.AddCell(commodityCell2);
                    }
                }

                if (!string.IsNullOrEmpty(li.Notes))
                {
                    var noteCell1 = InitializeCell("Notes:", _boldFont, colspan: 2, backgroundColor: _tableDataColor, bottomBorder: false);
                    table.AddCell(noteCell1);

                    var noteCell2 = InitializeCell(li.Notes, _font, colspan: 5, backgroundColor: _tableDataColor, bottomBorder: false);
                    table.AddCell(noteCell2);
                }
                if (!forVendor)
                {
                    if (order.HasLineSplits)
                    {
                        var accountTable = SplitAccountingInformation(li.Splits);
                        var acell = InitializeCell(colspan: 7);
                        acell.Add(accountTable);
                        table.AddCell(acell);
                    }
                }
            }
        }

        private Table SplitAccountingInformation(IEnumerable<Split> splits)
        {
            var table = InitializeTable(4);
            table.SetWidth(_pageWidth * .75f);

            table.AddCell(InitializeCell("Account", _tableHeaderFont, backgroundColor: _subTableHeaderColor, bottomBorder: false));
            table.AddCell(InitializeCell("Project", _tableHeaderFont, backgroundColor: _subTableHeaderColor, bottomBorder: false));
            table.AddCell(InitializeCell("Amount", _tableHeaderFont, backgroundColor: _subTableHeaderColor, bottomBorder: false));
            table.AddCell(InitializeCell("Distribution", _tableHeaderFont, backgroundColor: _subTableHeaderColor, bottomBorder: false));

            foreach (var split in splits)
            {
                table.AddCell(InitializeCell(split.FullAccountDisplay, _font, bottomBorder: false));
                table.AddCell(InitializeCell(split.Project, _font, bottomBorder: false));
                table.AddCell(InitializeCell(split.Amount.ToString("c"), _font, bottomBorder: false));

                if (split.LineItem == null)
                {
                    table.AddCell(InitializeCell((split.Amount / split.Order.GrandTotalFromDb).ToString("p"), _font, bottomBorder: false));
                }
                else
                {
                    table.AddCell(InitializeCell(((split.Amount / split.LineItem.TotalWithTax())).ToString("p"), _font, bottomBorder: false));
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

                accountingTable.AddCell(InitializeCell("Account:", _boldFont, bottomBorder: false, width: 1f));
                accountingTable.AddCell(InitializeCell(split.FullAccountDisplay, _font, bottomBorder: false, width: 4f));
                accountingTable.AddCell(InitializeCell("Project:", _boldFont, width: 1f));
                accountingTable.AddCell(InitializeCell(split.Project, _font, width: 4f));

                doc.Add(accountingTable);
            }
        }

        private void AddControlledSubstanceSection(Document doc, Order order)
        {
            if (order.HasControlledSubstance)
            {
                var table = InitializeTable(2);
                table.SetMarginBottom(1f);

                table.AddCell(InitializeCell("Controlled Substances Information", _tableHeaderFont, true, HorizontalAlignment.LEFT, colspan: 2, bottomBorder: true));


                var authorizationInfo = order.GetAuthorizationInfo();

                table.AddCell(InitializeCell("Class Schedule:", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 20));
                table.AddCell(InitializeCell(authorizationInfo.ClassSchedule, padding: false, bottomBorder: false, width: 80));

                table.AddCell(InitializeCell("Pharmaceutical Grade:", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 20));
                table.AddCell(InitializeCell(authorizationInfo.PharmaceuticalGrade == true ? "Yes" : "No", padding: false, bottomBorder: false, width: 80));

                table.AddCell(InitializeCell("Use:", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 20));
                table.AddCell(InitializeCell(authorizationInfo.Use, padding: false, bottomBorder: false, width: 80));

                table.AddCell(InitializeCell("Storage Site:", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 20));
                table.AddCell(InitializeCell(authorizationInfo.StorageSite, padding: false, bottomBorder: false, width: 80));

                table.AddCell(InitializeCell("Custodian:", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 20));
                table.AddCell(InitializeCell(authorizationInfo.Custodian, padding: false, bottomBorder: false, width: 80));

                table.AddCell(InitializeCell("End User:", _boldFont, padding: false, halignment: HorizontalAlignment.LEFT, bottomBorder: false, width: 20));
                table.AddCell(InitializeCell(authorizationInfo.EndUser, padding: false, bottomBorder: false, width: 80));

                table.AddCell(InitializeCell("", colspan: 2));
                doc.Add(table);
            }
        }

        private void AddComments(Document doc, Order order)
        {
            if (order.OrderComments.Any())
            {
                var cTable = InitializeTable(3);
                cTable.SetMarginTop(1f);

                cTable.AddCell(InitializeCell("Comments", _tableHeaderFont, true, HorizontalAlignment.LEFT, colspan: 3, bottomBorder: true));

                foreach (var c in order.OrderComments)
                {
                    cTable.AddCell(InitializeCell(c.DateCreated.ToString(), _font, bottomBorder: false, width: 1.5f));
                    cTable.AddCell(InitializeCell(c.User.FullName, _font, bottomBorder: false, width: 1.75f));
                    cTable.AddCell(InitializeCell(c.Text, _font, bottomBorder: false, width: 4f));
                }

                cTable.AddCell(InitializeCell("", colspan: 3));
                doc.Add(cTable);
            }
        }

        private void AddOrderHistory(Document doc, Order order)
        {


            var hTable = InitializeTable(4);
            hTable.SetMarginTop(1f);

            hTable.AddCell(InitializeCell("Order History", _tableHeaderFont, true, HorizontalAlignment.LEFT, colspan: 4, bottomBorder: false));

            foreach (var h in order.OrderTrackings)
            {
                hTable.AddCell(InitializeCell(h.DateCreated.ToString(), _font, bottomBorder: false, width: 1.5f));
                hTable.AddCell(InitializeCell(h.Description, _font, bottomBorder: false, width: 1.75f));
                hTable.AddCell(InitializeCell(h.StatusCode.Name, _font, bottomBorder: false, width: 2f));
                hTable.AddCell(InitializeCell(h.User.FullName, _font, bottomBorder: false, width: 2f));
            }

            hTable.AddCell(InitializeCell("", colspan: 4));
            doc.Add(hTable);

        }

        private Table InitializeTable(int columns = 1)
        {
            var table = new Table(columns)
                .UseAllAvailableWidth()
                .SetMarginBottom(2f);

            return table;
        }

        private Cell InitializeCell(
            string text = null, 
            PdfFont font = null, 
            bool header = false, 
            HorizontalAlignment? halignment = null, 
            VerticalAlignment? valignment = null, 
            int? colspan = null, 
            bool sideBorders = false, 
            bool topBottomBorders = false, 
            bool bottomBorder = true, 
            bool padding = true, 
            Color backgroundColor = null,
            float? width = null)
        {
            var cell = colspan.HasValue ? new Cell(1, colspan.Value) : new Cell();
            if (!string.IsNullOrEmpty(text)) cell = new Cell().Add(new Paragraph(text).SetFont(font ?? _font));

            if (header)
            {
                cell.SetBackgroundColor(_headerColor);
            }

            if (halignment.HasValue)
            {
                cell.SetHorizontalAlignment(halignment.Value);
            }

            if (valignment.HasValue)
            {
                cell.SetVerticalAlignment(valignment.Value);
            }

            if (backgroundColor != null)
            {
                cell.SetBackgroundColor(backgroundColor);
            }

            if (padding)
            {
                cell.SetPadding(5);
            }

            if (!sideBorders)
            {
                cell.SetBorderLeft(Border.NO_BORDER);
                cell.SetBorderRight(Border.NO_BORDER);
            }

            if (!topBottomBorders)
            {
                cell.SetBorderTop(Border.NO_BORDER);
                if (!bottomBorder)
                {
                    cell.SetBorderBottom(Border.NO_BORDER);
                }
            }

            if (width.HasValue)
            {
                cell.SetWidth(width.Value);
            }

            return cell;
        }
    }
}
