using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using etlMon.Common.Types.Response;

// ReSharper disable InconsistentNaming

namespace etlMon.Common.Types.Archie
{
    [CollectionDataContract(Namespace = "")]
    public class BOLs : List<BOL>
    {
    }

    [DataContract(Namespace = "")]
    public class BOL
    {

        public BOL()
        {
            ProcessingStatus = "";
            ProcessingReason = "";
            IsValid = false;
            HasTXT = false;
            HasPDF = false;
            HasXML = false;
            IsInsertedDocHeader = false;
            IsInsertedBOL = false;
            IsArchivedTXT = false;
            IsArchivedXML = false;
            IsArchivedPDF = false;
            IsExistingDoc = false;
            bolMCU = "";
            bolCarrier = "";
            bolLoadNum = string.Empty;
            bolCustNum = string.Empty;
            bolCustName = string.Empty;
            bolDocNum = string.Empty;
            bolBOLNum = string.Empty;
        }

        // Import Email properties

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public Guid AttachmentGuid { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public Guid EmailGuid { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public Guid ImportGuid { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public Int64 AttachmentId { get; set; }


        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string ProcessingStatus { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string ProcessingReason { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public bool IsValid { get; set; }


        // Attachment properties
        [DataMember(Order = 600, EmitDefaultValue = false)]
        public bool IsAttachmentPDF { get; set; }

        [DataMember(Order = 600, EmitDefaultValue = false)]
        public string AttachmentFileName { get; set; }

        [DataMember(Order = 600, EmitDefaultValue = false)]
        public string PDFFileName { get; set; }

        [DataMember(Order = 600, EmitDefaultValue = false)]
        public string InputFilePath { get; set; }

        [DataMember(Order = 600, EmitDefaultValue = false)]
        public string InputFileName { get; set; }

        [DataMember(Order = 600, EmitDefaultValue = false)]
        public string ErrorFilePath { get; set; }

        [DataMember(Order = 601, EmitDefaultValue = false)]
        public string OutPDFFilePath { get; set; }

        [DataMember(Order = 601, EmitDefaultValue = false)]
        public string OutPDFFileName { get; set; }

        //[DataMember(Order = 601, EmitDefaultValue = false)]
        //public string OutFilePath { get; set; }

        [DataMember(Order = 600, EmitDefaultValue = false)]
        public string OutTxtFilePath { get; set; }

        [DataMember(Order = 600, EmitDefaultValue = false)]
        public string OutTxtFileName { get; set; }

        [DataMember(Order = 600, EmitDefaultValue = false)]
        public string OutXMLFilePath { get; set; }

        [DataMember(Order = 600, EmitDefaultValue = false)]
        public string OutXMLFileName { get; set; }


        // TEXT FILE VALUES
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public int DocID { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public int SatID { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public DateTime DateStored { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public DateTime? DateDelete { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string DocType { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public DateTime? OfflineDate { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = false)]
        public int? DeleteFlag { get; set; }

        [DataMember(Order = 80, EmitDefaultValue = false)]
        public int? FS_InboxOwner { get; set; }

        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string FS_InboxSubject { get; set; }

        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string FS_ReprintServer { get; set; }

        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string FS_ReprintPrinter { get; set; }

        // Raw properties from txt file
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public string BOLCUSTNUM { get; set; }

        [DataMember(Order = 140, EmitDefaultValue = false)]
        public string BOLCUSTNAM { get; set; }

        [DataMember(Order = 150, EmitDefaultValue = false)]
        public string BOLDATE { get; set; }

        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string BOLLOADNO { get; set; }

        [DataMember(Order = 170, EmitDefaultValue = false)]
        public string BOLCARRIER { get; set; }

        [DataMember(Order = 180, EmitDefaultValue = false)]
        public string BOLBOLNUM { get; set; }

        [DataMember(Order = 190, EmitDefaultValue = false)]
        public string BOLDOCNUM { get; set; }


        // Strongly typed properties
        [DataMember(Order = 300, EmitDefaultValue = false)]
        public string bolCustNum { get; set; }

        [DataMember(Order = 310, EmitDefaultValue = false)]
        public string bolCustName { get; set; }

        [DataMember(Order = 320, EmitDefaultValue = false)]
        public DateTime bolDateCalc { get; set; }

        [DataMember(Order = 330, EmitDefaultValue = false)]
        public string bolLoadNum { get; set; }

        [DataMember(Order = 340, EmitDefaultValue = false)]
        public string bolCarrier { get; set; }

        [DataMember(Order = 350, EmitDefaultValue = false)]
        public string bolBOLNum { get; set; }

        [DataMember(Order = 360, EmitDefaultValue = false)]
        public string bolDocNum { get; set; }

        [DataMember(Order = 370, EmitDefaultValue = false)]
        public string bolMCU { get; set; }

        [DataMember(Order = 380, EmitDefaultValue = false)]
        public string bolDate { get; set; }

        [DataMember(Order = 390, EmitDefaultValue = false)]
        public string bolSubject { get; set; }

        [DataMember(Order = 390, EmitDefaultValue = false)]
        public string bolDocStamp { get; set; }

        [DataMember(Order = 390, EmitDefaultValue = false)]
        public string bolDocTitle { get; set; }

        // TCC SQL Strongly typed properties
        [DataMember(Order = 300, EmitDefaultValue = false)]
        public string DOCNUMBER2 { get; set; } // bolCustNum

        [DataMember(Order = 310, EmitDefaultValue = false)]
        public string DOCDESC1 { get; set; } // bolCustName

        [DataMember(Order = 320, EmitDefaultValue = false)]
        public DateTime DOCDATE_CALC { get; set; } // bolDateCalc

        [DataMember(Order = 330, EmitDefaultValue = false)]
        public string LOADNUM { get; set; } // bolLoadNum

        [DataMember(Order = 340, EmitDefaultValue = false)]
        public string CARRIER { get; set; } // bolCarrier

        [DataMember(Order = 350, EmitDefaultValue = false)]
        public string BOLNum { get; set; } // bolBOLNum

        [DataMember(Order = 360, EmitDefaultValue = false)]
        public string DOCNUMBER1 { get; set; } // bolDocNum

        [DataMember(Order = 370, EmitDefaultValue = false)]
        public string DOCLOCATION { get; set; } // bolMCU

        [DataMember(Order = 380, EmitDefaultValue = false)]
        public string DOCDATE { get; set; } // bolDate

        [DataMember(Order = 380, EmitDefaultValue = false)]
        public string DOCDATE_STR { get; set; } // bolDate

        [DataMember(Order = 390, EmitDefaultValue = false)]
        public string DOCSUBJECT { get; set; } // bolSubject

        [DataMember(Order = 390, EmitDefaultValue = false)]
        public string DOCSTAMP { get; set; } // bolDocStamp

        //[DataMember(Order = 390, EmitDefaultValue = false)]
        //public string bolDocTitle { get; set; } // bolDocTitle



        [DataMember(Order = 620, EmitDefaultValue = false)]
        public int Version { get; set; }
        [DataMember(Order = 630, EmitDefaultValue = false)]
        public byte[] Image { get; set; }
        [DataMember(Order = 640, EmitDefaultValue = false)]
        public string OfflinePath { get; set; }
        [DataMember(Order = 650, EmitDefaultValue = false)]
        public int? OfflineFlag { get; set; }
        [DataMember(Order = 660, EmitDefaultValue = false)]
        public int CatID { get; set; }


        [DataMember(Order = 600, EmitDefaultValue = false)]
        public byte[] PDF { get; set; }

        [DataMember(Order = 610, EmitDefaultValue = false)]
        public bool HasTXT { get; set; }

        [DataMember(Order = 615, EmitDefaultValue = false)]
        public bool HasXML { get; set; }

        [DataMember(Order = 620, EmitDefaultValue = false)]
        public bool HasPDF { get; set; }

        [DataMember(Order = 630, EmitDefaultValue = false)]
        public ValidationError ValidationError { get; set; }

        [DataMember(Order = 640, EmitDefaultValue = false)]
        public bool IsInsertedDocHeader { get; set; }

        [DataMember(Order = 650, EmitDefaultValue = false)]
        public bool IsInsertedBOL { get; set; }

        [DataMember(Order = 660, EmitDefaultValue = false)]
        public bool IsArchivedTXT { get; set; }

        [DataMember(Order = 665, EmitDefaultValue = false)]
        public bool IsArchivedXML { get; set; }
        [DataMember(Order = 670, EmitDefaultValue = false)]
        public bool IsArchivedPDF { get; set; }

        [DataMember(Order = 680, EmitDefaultValue = false)]
        public string DontKnow { get; set; }

        [DataMember(Order = 690, EmitDefaultValue = false)]
        public bool IsExistingDoc { get; set; }

    }
}
