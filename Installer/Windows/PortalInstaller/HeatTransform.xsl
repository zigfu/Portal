<?xml version="1.0" ?>
<xsl:transform version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">

  <!-- By default, copy all attributes and elements to the output. -->
  <xsl:template match="@*|*">
    <xsl:copy>
      <xsl:apply-templates select="@*" />
      <xsl:apply-templates select="*" />
    </xsl:copy>
  </xsl:template>

  <!-- Override copy (do nothing) for elements with this Id, so they are omitted from the output. -->
  <xsl:template match='wix:Component[wix:File/@Source="$(var.PortalOut)\bin\Portal.exe"]' />

</xsl:transform>
