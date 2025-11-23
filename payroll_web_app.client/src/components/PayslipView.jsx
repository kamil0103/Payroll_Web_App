import React, { useEffect, useRef, useState } from "react";
import jsPDF from "jspdf";
import html2canvas from "html2canvas";

export default function PayslipView({ payrollId, token }) {
  const [payslip, setPayslip] = useState(null);
  const [loading, setLoading] = useState(true);
  const slipRef = useRef();

  useEffect(() => {
    if (!payrollId) return;
    const fetchPayslip = async () => {
      setLoading(true);
      const res = await fetch(`/payroll/${payrollId}/payslip`, {
        headers: {
          Authorization: token ? `Bearer ${token}` : undefined,
        },
      });
      if (res.ok) {
        const json = await res.json();
        setPayslip(json);
      } else {
        console.error("Failed to load payslip", res.status);
      }
      setLoading(false);
    };
    fetchPayslip();
  }, [payrollId, token]);

  const exportPdf = async () => {
    if (!slipRef.current) return;
    const element = slipRef.current;

    // use html2canvas to render element
    const canvas = await html2canvas(element, { scale: 2 });
    const imgData = canvas.toDataURL("image/png");

    const pdf = new jsPDF("p", "mm", "a4");
    const pageWidth = pdf.internal.pageSize.getWidth();
    const pageHeight = pdf.internal.pageSize.getHeight();

    // compute image dims to fit width
    const imgProps = pdf.getImageProperties(imgData);
    const imgWidth = pageWidth;
    const imgHeight = (imgProps.height * imgWidth) / imgProps.width;

    let position = 0;
    pdf.addImage(imgData, "PNG", 0, position, imgWidth, imgHeight);
    // If the content is taller than a page, additional pages handling:
    if (imgHeight > pageHeight) {
      let heightLeft = imgHeight - pageHeight;
      while (heightLeft > 0) {
        position = position - pageHeight;
        pdf.addPage();
        pdf.addImage(imgData, "PNG", 0, position, imgWidth, imgHeight);
        heightLeft -= pageHeight;
      }
    }

    pdf.save(`payslip-${payrollId}.pdf`);
  };

  if (loading) return <div>Loading payslip...</div>;
  if (!payslip) return <div>Payslip not found.</div>;

  return (
    <div>
      <div style={{ marginBottom: 12 }}>
        <button onClick={exportPdf}>Export as PDF</button>
        <a href={`/payroll/${payrollId}/payslip/html`} target="_blank" rel="noreferrer" style={{ marginLeft: 8 }}>
          Open HTML (preview)
        </a>
      </div>

      <div ref={slipRef} style={{ width: "800px", padding: 20, border: "1px solid #ddd", background: "#fff", color: "#222" }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <h2>Payslip</h2>
          <div>Generated: {new Date(payslip.generatedAt).toLocaleString()}</div>
        </div>

        <div style={{ marginTop: 8, marginBottom: 8 }}>
          <div><strong>Employee:</strong> {payslip.employeeName}</div>
          <div><strong>Department:</strong> {payslip.department ?? ""} <strong style={{ marginLeft: 12 }}>Job:</strong> {payslip.jobTitle ?? ""}</div>
        </div>

        <table style={{ width: "100%", borderCollapse: "collapse" }}>
          <tbody>
            <tr>
              <td style={{ padding: 6, borderBottom: "1px solid #eee" }}>Pay Period</td>
              <td style={{ padding: 6, textAlign: "right", borderBottom: "1px solid #eee" }}>
                {new Date(payslip.payPeriodStart).toLocaleDateString()} → {new Date(payslip.payPeriodEnd).toLocaleDateString()}
              </td>
            </tr>
            <tr>
              <td style={{ padding: 6, borderBottom: "1px solid #eee" }}>Total Hours</td>
              <td style={{ padding: 6, textAlign: "right", borderBottom: "1px solid #eee" }}>{payslip.totalHours}</td>
            </tr>
            <tr>
              <td style={{ padding: 6, borderBottom: "1px solid #eee" }}>Gross Pay</td>
              <td style={{ padding: 6, textAlign: "right", borderBottom: "1px solid #eee" }}>{payslip.grossPay.toLocaleString(undefined, { style: "currency", currency: "USD" })}</td>
            </tr>
            <tr>
              <td style={{ padding: 6, borderBottom: "1px solid #eee" }}>Tax</td>
              <td style={{ padding: 6, textAlign: "right", borderBottom: "1px solid #eee" }}>-{payslip.taxDeductions.toLocaleString(undefined, { style: "currency", currency: "USD" })}</td>
            </tr>
            <tr>
              <td style={{ padding: 6, borderBottom: "1px solid #eee" }}>Benefits</td>
              <td style={{ padding: 6, textAlign: "right", borderBottom: "1px solid #eee" }}>-{payslip.benefitsDeductions.toLocaleString(undefined, { style: "currency", currency: "USD" })}</td>
            </tr>
            <tr>
              <td style={{ padding: 6, borderBottom: "1px solid #eee" }}><strong>Net Pay</strong></td>
              <td style={{ padding: 6, textAlign: "right", borderBottom: "1px solid #eee" }}><strong>{payslip.netPay.toLocaleString(undefined, { style: "currency", currency: "USD" })}</strong></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  );
}