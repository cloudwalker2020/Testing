using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    internal class Program
    {
        public const char CategoryTrade = 'T';
        public const char CategoryControl = 'C';
        public const char CategoryAdmin = 'A';

        private static void Main(string[] args)
        {
            var lines = File.ReadAllText("spds.txt")
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var bytes = Encoding.ASCII.GetBytes(line);

                var reader = new BinaryReader(new MemoryStream(bytes), Encoding.ASCII);

                var header = new MessageHeader(reader);

                switch (header.MessageCategory)
                {
                    case 'T':
                        HandleTrade(header, reader);
                        break;
                    case 'C':
                        break;
                    case 'A':
                        break; 
                }

            }
        }

        private static void HandleTrade(MessageHeader header, BinaryReader reader)
        {
            switch (header.MessageType)
            {
                case 'M':
                    var tradeReport = new TradeReport(reader);
                    break;
                case 'P':
                    var mbsTrade = new MBSTradeReport(reader);
                    break;
                case 'N':
                    var tradeCancel = new TradeCancel(reader);
                    break;
                case 'Q':
                    var mbsCancel = new MBSTradeCancel(reader);
                    break;
                case 'O':
                    var tradeCorrection = new TradeCorrection(reader);
                    break;
                case 'R':
                    var mbsTradeCorrection = new MBSTradeCorrection(reader);
                    break;
            }
        }

        private static void HandleTradeReport(MessageHeader header, BinaryReader reader)
        {

             




        }
    }



    //27 Bytes
    internal struct MessageHeader
    {
        public char MessageCategory;
        public char MessageType;
        public char Reserved;
        public string RetransmissionRequester; //2 bytes
        public string MessageSequenceNumber; //7 bytes
        public char MarketCenter;

        public TraceDateTime Date;

        public MessageHeader(BinaryReader reader)
        {
            MessageCategory = reader.ReadChar();
            MessageType = reader.ReadChar();
            Reserved = reader.ReadChar();
            RetransmissionRequester = Encoding.ASCII.GetString(reader.ReadBytes(2));
            MessageSequenceNumber = Encoding.ASCII.GetString(reader.ReadBytes(7));
            //var str = Encoding.ASCII.GetString(reader.ReadBytes(7));
            //int.TryParse(str, out MessageSequenceNumber);
            MarketCenter = reader.ReadChar();

            Date = new TraceDateTime(reader);
        }
    }

    //8 Bytes
    internal struct TraceDate
    {
        public TraceDate(BinaryReader reader)
        {
            Year = Encoding.ASCII.GetString(reader.ReadBytes(4));
            Month = Encoding.ASCII.GetString(reader.ReadBytes(2));
            Day = Encoding.ASCII.GetString(reader.ReadBytes(2));
        }

        public string Year; //4 bytes
        public string Month; //2 bytes
        public string Day; //2 bytes
    }

    //14 Bytes
    internal struct TraceDateTime
    {
        public TraceDateTime(BinaryReader reader)
        {
            Date = new TraceDate(reader);
            Hour = Encoding.ASCII.GetString(reader.ReadBytes(2));
            Minute = Encoding.ASCII.GetString(reader.ReadBytes(2));
            Seconds = Encoding.ASCII.GetString(reader.ReadBytes(2));
        }

        public TraceDate Date; // 8 bytes
        public string Hour; //2 bytes
        public string Minute; //2 bytes
        public string Seconds; //2 bytes
    }

    //40 Bytes
    internal struct TradeLabel
    {
        public string Symbol; //14
        public string Cusip; //9
        public string BSYM; //12
        public string SubProductType; //5

        public TradeLabel(BinaryReader reader)
        {
            Symbol = Encoding.ASCII.GetString(reader.ReadBytes(14));
            Cusip = Encoding.ASCII.GetString(reader.ReadBytes(9));
            BSYM = Encoding.ASCII.GetString(reader.ReadBytes(12));
            SubProductType = Encoding.ASCII.GetString(reader.ReadBytes(5));
        }
    }

    //71 Bytes
    struct TradeInformation
    {
        public TradeInformation(BinaryReader reader)
        {
            QuantityIndicator = reader.ReadChar();
            Quantity = Encoding.ASCII.GetString(reader.ReadBytes(14));
            Price = Encoding.ASCII.GetString(reader.ReadBytes(11));
            Remuneration = reader.ReadChar();
            SpecialPriceIndicator = reader.ReadChar();
            Side = reader.ReadChar();
            AsOfIndicator = reader.ReadChar();
            ExecutionDateTime = new TraceDateTime(reader);
            FutureUse = Encoding.ASCII.GetString(reader.ReadBytes(2));
            SaleCondition3 = reader.ReadChar();
            SaleCondition4 = reader.ReadChar();
            SettlementDate = Encoding.ASCII.GetString(reader.ReadBytes(8));
            Factor = Encoding.ASCII.GetString(reader.ReadBytes(12));
            ReportingPartyType = reader.ReadChar();
            ContraPartyType = reader.ReadChar();
            ATSIndicator = reader.ReadChar();
        }

        public char QuantityIndicator; //1
        public string Quantity; //14
        public string Price; //11
        public char Remuneration; //1
        public char SpecialPriceIndicator; //1
        public char Side; //1
        public char AsOfIndicator; //1
        public TraceDateTime ExecutionDateTime; //14
        public string FutureUse; //2
        public char SaleCondition3; //1
        public char SaleCondition4; //1
        public string SettlementDate; //8
        public string Factor; //12
        public char ReportingPartyType; //1
        public char ContraPartyType; //1
        public char ATSIndicator; //1
    }

    // 120 Bytes
    struct TradeReport
    {
        public TradeReport(BinaryReader reader)
        {
            Label = new TradeLabel(reader);
            OriginalDisseminationDate = new TraceDate(reader);
            TradeInformation = new TradeInformation(reader);
            ChangeIndicator = reader.ReadChar();
        }

        public TradeLabel Label;
        public TraceDate OriginalDisseminationDate;
        public TradeInformation TradeInformation;
        public char ChangeIndicator;
    }

    //30 Bytes
    struct MBSTradeLabel
    {
        public MBSTradeLabel(BinaryReader reader)
        {
            RDID = Encoding.ASCII.GetString(reader.ReadBytes(25));
            SubProductType = Encoding.ASCII.GetString(reader.ReadBytes(5));
        }

        public string RDID; //25
        public string SubProductType; //5
    }

    //59 Bytes
    struct MBSTradeInformation
    {
        public char QuantityIndicator; //1
        public string Quantity; //14
        public string Price; //11
        public char Remuneration; //1
        public char SpecialPriceIndicator; //1
        public char Side; //1
        public char AsOfIndicator; //1
        public TraceDateTime ExecutionDateTime; //14
        public string FutureUse; //2
        public char SaleCondition3; //1
        public char SaleCondition4; //1
        public string SettlementDate; //8
        public char ReportingPartyType; //1
        public char ContraPartyType; //1
        public char ATSIndicator; //1

        public MBSTradeInformation(BinaryReader reader)
        {
            QuantityIndicator = reader.ReadChar();
            Quantity = Encoding.ASCII.GetString(reader.ReadBytes(14));
            Price = Encoding.ASCII.GetString(reader.ReadBytes(11));
            Remuneration = reader.ReadChar();
            SpecialPriceIndicator = reader.ReadChar();
            Side = reader.ReadChar();
            AsOfIndicator = reader.ReadChar();
            ExecutionDateTime = new TraceDateTime(reader);
            FutureUse = Encoding.ASCII.GetString(reader.ReadBytes(2));
            SaleCondition3 = reader.ReadChar();
            SaleCondition4 = reader.ReadChar();
            SettlementDate = Encoding.ASCII.GetString(reader.ReadBytes(8));
            ReportingPartyType = reader.ReadChar();
            ContraPartyType = reader.ReadChar();
            ATSIndicator = reader.ReadChar();
        }
    }

    //98 bytes
    struct MBSTradeReport
    {
        public MBSTradeLabel TradeLabel;
        public TraceDate OriginalDisseminationDate;
        public MBSTradeInformation TradeInformation;
        public char ChangeIndicator;

        public MBSTradeReport(BinaryReader reader)
        {
            TradeLabel = new MBSTradeLabel(reader);
            OriginalDisseminationDate = new TraceDate(reader);
            TradeInformation = new MBSTradeInformation(reader);
            ChangeIndicator = reader.ReadChar();
        }
    }

    //34 bytes
    internal struct SummaryInformation
    {
        public SummaryInformation(BinaryReader reader)
        {
            this.HighPrice = Encoding.ASCII.GetString(reader.ReadBytes(11));
            this.LowPrice = Encoding.ASCII.GetString(reader.ReadBytes(11));
            this.LastSalePrice = Encoding.ASCII.GetString(reader.ReadBytes(11));
            this.ChangeIndicator = reader.ReadChar();
        }

        public string HighPrice; //11
        public string LowPrice;//11
        public string LastSalePrice;//11
        public char ChangeIndicator;//1
    }

    //161 bytes
    struct TradeCancel
    {
        public TradeLabel Label;
        public TraceDate OriginalDisseminationDate;
        public string OriginalSequenceNumber;
        public char Function;
        public TradeInformation OriginalTradeInformation;
        public SummaryInformation SummaryInformation;

        public TradeCancel(BinaryReader reader)
        {
            Label = new TradeLabel(reader);
            OriginalDisseminationDate = new TraceDate(reader);
            OriginalSequenceNumber = Encoding.ASCII.GetString(reader.ReadBytes(7));
            Function = reader.ReadChar();
            OriginalTradeInformation = new TradeInformation(reader);
            SummaryInformation = new SummaryInformation(reader);
        }
    }


    //139 Bytes
    struct MBSTradeCancel
    {
        public MBSTradeLabel Label;
        public TraceDate OriginalDisseminationDate;
        public string OriginalSequenceNumber;
        public char Function;
        public MBSTradeInformation OriginalTradeInformation;
        public SummaryInformation SummaryInformation;

        public MBSTradeCancel(BinaryReader reader)
        {
            Label = new MBSTradeLabel(reader);
            OriginalDisseminationDate = new TraceDate(reader);
            OriginalSequenceNumber = Encoding.ASCII.GetString(reader.ReadBytes(7));
            Function = reader.ReadChar();
            OriginalTradeInformation = new MBSTradeInformation(reader);
            SummaryInformation = new SummaryInformation(reader);
        }
    }

    //232 bytes
    struct TradeCorrection
    {
        public TradeLabel Label;
        public TraceDate OriginalDisseminationDate;
        public string OriginalSequenceNumber;
        public char Function;
        public TradeInformation OriginalTradeInformation;
        public TradeInformation TradeInformation;
        public SummaryInformation SummaryInformation;

        public TradeCorrection(BinaryReader reader)
        {
            Label = new TradeLabel(reader);
            OriginalDisseminationDate = new TraceDate(reader);
            OriginalSequenceNumber = Encoding.ASCII.GetString(reader.ReadBytes(7));
            Function = reader.ReadChar();
            OriginalTradeInformation = new TradeInformation(reader);
            TradeInformation = new TradeInformation(reader);
            SummaryInformation = new SummaryInformation(reader);
        }
    }

    //198 Bytes
    struct MBSTradeCorrection
    {
        public MBSTradeLabel Label;
        public TraceDate OriginalDisseminationDate;
        public string OriginalSequenceNumber;
        public char Function;
        public MBSTradeInformation OriginalTradeInformation;
        public MBSTradeInformation TradeInformation;
        public SummaryInformation SummaryInformation;

        public MBSTradeCorrection(BinaryReader reader)
        {
            Label = new MBSTradeLabel(reader);
            OriginalDisseminationDate = new TraceDate(reader);
            OriginalSequenceNumber = Encoding.ASCII.GetString(reader.ReadBytes(7));
            Function = reader.ReadChar();
            OriginalTradeInformation = new MBSTradeInformation(reader);
            TradeInformation = new MBSTradeInformation(reader);
            SummaryInformation = new SummaryInformation(reader);
        }
    }
}
