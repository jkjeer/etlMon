<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:ms="urn:schemas-microsoft-com:xslt">

  <xsl:template match="root">
    <html>
      <head>
        <style>
          body,td,p,th  { font-family: arial; font-size: 8pt; }
          tr.head { border : 0px solid black; padding: 0 0px; background: #ddddff; font-weight:bold;}
          td.head { border : 0px solid black; padding: 0 0px; background: #ddddff; }
          td.numeric { text-align:right; width:80px;  }
          td.noBorder {border: none; }
          .error {color: red;}
          .title {font-weight:bold; text-decoration: underline;}
          .depth1 {text-indent: 0px;}
          .depth2 {text-indent: 5px;}
          .depth3 {text-indent: 10px;}
          .depth4 {text-indent: 15px;}
          .depth5 {text-indent: 20px;}
          .depth6 {text-indent: 25px;}
          .depth7 {text-indent: 30px;}
          .depth8 {text-indent: 35px;}
          .depth9 {text-indent: 40px;}
          .depth10 {text-indent: 45px;}
          .depth11 {text-indent: 50px;}
          .depth12 {text-indent: 55px;}
          .depth13 {text-indent: 60px;}
          .depth14 {text-indent: 65px;}
          .depth15 {text-indent: 70px;}
          tr.odd { background: #ffffff; }
          tr.even { background: #D5CFCF; }
          tr.stepodd { background: #ffffcc; }
          tr.stepeven { background: #e6ffe6; }
          .defaultTable td
          {
          border: 1px solid black;
          border-collapse:collapse;
          padding:2px;
          }
        </style>
      </head>
      <body>
        <div class="title">TCC Export Invoices</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">Start Time</td>
            <td align="left">End Time</td>
            <td align="left">Elapsed Seconds</td>
            <td align="left">Exported Invoices</td>
            <td align="left">Export Guid</td>
          </tr>
          <xsl:apply-templates select="SummaryNode">
          </xsl:apply-templates>
        </table>

        <br/>

        <xsl:if test="QueryNodeCustDate">
        <div class="title">Query</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">User</td>
            <td align="left">Location</td>
            <td align="left">Customer</td>
            <td align="left">From Date</td>
            <td align="left">To Date</td>
            <td align="left">Invoices</td>
            <td align="left">Email</td>
          </tr>
          <xsl:apply-templates select="QueryNodeCustDate/Query">
          </xsl:apply-templates>
        </table>
        </xsl:if>

        <xsl:if test="QueryNodeInvoiceRange">
          <div class="title">Query</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">User</td>
              <td align="left">Location</td>
              <td align="left">From Invoice#</td>
              <td align="left">To Invoice#</td>
              <td align="left">Invoices</td>
              <td align="left">Email</td>
            </tr>
            <xsl:apply-templates select="QueryNodeInvoiceRange/Query">
            </xsl:apply-templates>
          </table>
        </xsl:if>

        <xsl:if test="QueryNodeInvoiceList">
          <div class="title">Query</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">User</td>
              <td align="left">Location</td>
              <td align="left">Invoice List</td>
              <td align="left">Invoices</td>
              <td align="left">Email</td>
            </tr>
            <xsl:apply-templates select="QueryNodeInvoiceList/Query">
            </xsl:apply-templates>
          </table>
        </xsl:if>

        <xsl:if test="ActionLinks/ArrayOfActionLink/ActionLink">
          <br/>
          <div class="title">Exported Invoice Action Links</div>
          <ul>
            <xsl:apply-templates select="ActionLinks/ArrayOfActionLink/ActionLink">
            </xsl:apply-templates>
          </ul>
        </xsl:if>

        <xsl:if test="//Error">
          <br/>
          <div class="title">Errors</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Message</td>
              <td align="left">Source</td>
              <td align="left">Stack Trace</td>
            </tr>
            <xsl:apply-templates select="//Error">
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>

        <xsl:if test="ProcessResponse/Messages/Message">
          <br/>
          <div class="title">Messages</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Name</td>
              <td align="left">Value</td>
            </tr>
            <xsl:apply-templates select="ProcessResponse/Messages/Message"/>
          </table>
          <br/>
        </xsl:if>

        <xsl:if test="ResultLine/ResultLines/ResultLine">
          <br/>
          <div class="title">Exported Invoices</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">DocSubject</td>
              <td align="left">DocNum</td>
              <td align="left">CustNum</td>
              <td align="left">CustName</td>
              <td align="left">DocDate</td>
              <td align="left">LoadNum</td>
              <td align="left">BOL</td>
              <td align="left">PO</td>
              <td align="left">PDFPath</td>
            </tr>
            <xsl:apply-templates select="ResultLine/ResultLines/ResultLine"/>
          </table>
        </xsl:if>

        <xsl:if test="ProcessResponse">
          <br/>
          <div class="title">Process Steps</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Name</td>
              <td align="left">Start Date</td>
              <td align="left">End Date</td>
              <td align="left">Elapsed(Sec)</td>
              <td align="left">Rows Affected</td>
              <td align="left">Messages</td>
            </tr>
            <xsl:apply-templates select="ProcessResponse">
              <xsl:sort select="StartDate"/>
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>

        <xsl:if test="ProcessResponse/ConnectionStrings">
          <div class="title">Connection Strings</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Name</td>
              <td align="left">Value</td>
            </tr>
            <xsl:apply-templates select="ProcessResponse/ConnectionStrings/ConnectionString">
              <xsl:sort select="Name"/>
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>

        <br/>
        <table class="defaultTable">
          <tr>
            <td>Assembly Version</td>
            <td>
              <xsl:value-of select="//AssemblyVersion"/>
            </td>
          </tr>
          <tr>
            <td>User Name</td>
            <td>
              <xsl:value-of select="//ServiceContext/UserName"/>
            </td>
          </tr>
          <tr>
            <td>Session Id</td>
            <td>
              <xsl:value-of select="//ServiceContext/SessionId"/>
            </td>
          </tr>
          <tr>
            <td>Server Address</td>
            <td>
              <xsl:value-of select="//ServiceContext/ServerAddress"/>
            </td>
          </tr>
        </table>

      </body>
    </html>
  </xsl:template>

  <xsl:template match="SummaryNode/Header">
    <tr>
      <td>
        <xsl:value-of select="StartDateString"/>
      </td>
      <td>
        <xsl:value-of select="EndDateString"/>
      </td>
      <td align="right">
        <xsl:value-of select="format-number(ElapsedSeconds, '#,##0.00')"/>
      </td>
      <td align="right">
        <xsl:value-of select="RowsAffected"/>
      </td>
      <td>
        <xsl:value-of select="exportGUID"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="QueryNodeCustDate/Query">
    <tr>
      <td>
        <xsl:value-of select="user"/>
      </td>
      <td>
        <xsl:value-of select="plant"/>
      </td>
      <td>
        <xsl:value-of select="Customer"/>
      </td>
      <td>
        <xsl:value-of select="StartDateString"/>
      </td>
      <td>
        <xsl:value-of select="EndDateString"/>
      </td>
      <td align="right">
        <xsl:value-of select="RowsAffected"/>
      </td>
      <td>
        <xsl:value-of select="email"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="QueryNodeInvoiceRange/Query">
    <tr>
      <td>
        <xsl:value-of select="user"/>
      </td>
      <td>
        <xsl:value-of select="plant"/>
      </td>
      <td>
        <xsl:value-of select="startInvoice"/>
      </td>
      <td>
        <xsl:value-of select="endInvoice"/>
      </td>
      <td align="right">
        <xsl:value-of select="RowsAffected"/>
      </td>
      <td>
        <xsl:value-of select="email"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="QueryNodeInvoiceList/Query">
    <tr>
      <td>
        <xsl:value-of select="user"/>
      </td>
      <td>
        <xsl:value-of select="plant"/>
      </td>
      <td>
        <xsl:value-of select="invoiceList"/>
      </td>
      <td align="right">
        <xsl:value-of select="RowsAffected"/>
      </td>
      <td>
        <xsl:value-of select="email"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="ActionLink">
    <li>
      <xsl:element name="a">
        <xsl:attribute name="href">
          <xsl:value-of select="Href"/>
        </xsl:attribute>
        <xsl:value-of select="Description"/>
      </xsl:element>
    </li>
  </xsl:template>

  <xsl:template match="ProcessResponse">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td class="depth{count(ancestor::*)}">
        <xsl:value-of select="ProcessName"/>
      </td>
      <td>
        <xsl:value-of select="StartDate"/>
      </td>
      <td>
        <xsl:value-of select="EndDate"/>
      </td>
      <td align="right">
        <xsl:value-of select="format-number(ElapsedSeconds, '#,##0.00')"/>
      </td>
      <td align="right">
        <xsl:value-of select="RowsAffected"/>
      </td>
      <td>
        <xsl:value-of select="Errors/Error/Message"/>
      </td>
    </tr>
    <xsl:if test="ProcessSteps/ProcessStep">
      <xsl:apply-templates select="ProcessSteps/ProcessStep">
        <xsl:sort select="StartDate"/>
      </xsl:apply-templates>
    </xsl:if>
  </xsl:template>

  <xsl:template match="ProcessStep">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">stepeven</xsl:when>
          <xsl:otherwise>stepeven</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td class="depth{count(ancestor::*)}">
        <xsl:value-of select="Name"/>
      </td>
      <td>
        <xsl:value-of select="StartDate"/>
      </td>
      <td>
        <xsl:value-of select="EndDate"/>
      </td>
      <td align="right">
        <xsl:value-of select="format-number(ElapsedSeconds, '#,##0.00')"/>
      </td>
      <td align="right">
        <xsl:value-of select="RowsAffected"/>
      </td>
      <td>
        <xsl:value-of select="Errors/Error/Message"/>
      </td>
    </tr>
    <xsl:if test="ProcessResponses">
      <xsl:apply-templates select="ProcessResponses/ProcessResponse">
        <xsl:sort select="StartDate"/>
      </xsl:apply-templates>
    </xsl:if>
  </xsl:template>

  <xsl:template match="ConnectionString">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="Name"/>
      </td>
      <td>
        <xsl:value-of select="Value"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="//Error">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="Message"/>
      </td>
      <td>
        <xsl:value-of select="Source"/>
      </td>
      <td>
        <xsl:value-of select="StackTrace"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="Message">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="Name"/>
      </td>
      <td>
        <xsl:value-of select="Value"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="ResultLine/ResultLines/ResultLine">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="DOCSUBJECT"/>
      </td>
      <td>
        <xsl:value-of select="DOCNUMBER1"/>
      </td>
      <td>
        <xsl:value-of select="DOCNUMBER2"/>
      </td>
      <td>
        <xsl:value-of select="DOCDESC1"/>
      </td>
      <td>
        <xsl:value-of select="DOCDATE_STR"/>
      </td>
      <td>
        <xsl:value-of select="LOADNUM"/>
      </td>
      <td>
        <xsl:value-of select="BOL"/>
      </td>
      <td>
        <xsl:value-of select="PO1"/>
      </td>
      <td>
        <xsl:value-of select="ExportPDFPath"/>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>

