(() => {
    const pdfColors = {
        navy: [0.10, 0.20, 0.31],
        gold: [0.79, 0.53, 0.20],
        ink: [0.15, 0.19, 0.24],
        muted: [0.31, 0.36, 0.41],
        line: [0.85, 0.88, 0.91],
        stripe: [0.96, 0.97, 0.98],
        white: [1, 1, 1],
        green: [0.08, 0.40, 0.28],
        red: [0.68, 0.20, 0.18]
    };

    const cp1252 = new Map([
        ["€", 0x80], ["‚", 0x82], ["ƒ", 0x83], ["„", 0x84], ["…", 0x85], ["†", 0x86], ["‡", 0x87],
        ["ˆ", 0x88], ["‰", 0x89], ["Š", 0x8a], ["‹", 0x8b], ["Œ", 0x8c], ["Ž", 0x8e], ["‘", 0x91],
        ["’", 0x92], ["“", 0x93], ["”", 0x94], ["•", 0x95], ["–", 0x96], ["—", 0x97], ["™", 0x99],
        ["š", 0x9a], ["›", 0x9b], ["œ", 0x9c], ["ž", 0x9e], ["Ÿ", 0x9f]
    ]);

    function cleanText(value) {
        return value == null || value === "" ? "—" : String(value).replace(/[\r\n\t]+/g, " ").trim();
    }

    function money(value) {
        return value == null || value === "" ? "—" : `$ ${new Intl.NumberFormat("es-CO", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(Number(value))}`;
    }

    function dateLabel(value) {
        if (!value) return "—";
        const date = new Date(value);
        return Number.isNaN(date.getTime()) ? "—" : new Intl.DateTimeFormat("es-CO", { dateStyle: "medium", timeStyle: "short" }).format(date);
    }

    function safeFileDate() {
        const d = new Date();
        return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;
    }

    function saveBlob(bytes, type, filename) {
        const blob = new Blob([bytes], { type });
        const url = URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = filename;
        link.style.display = "none";
        document.body.appendChild(link);
        link.click();
        link.remove();
        setTimeout(() => URL.revokeObjectURL(url), 1000);
    }

    function pdfLiteral(value) {
        let out = "";
        for (const ch of String(value ?? "")) {
            let code = ch.codePointAt(0);
            if (code > 255) code = cp1252.get(ch) ?? 0x3f;
            if (ch === "\\" || ch === "(" || ch === ")") out += `\\${ch}`;
            else if (code < 32 || code > 126) out += `\\${code.toString(8).padStart(3, "0")}`;
            else out += String.fromCharCode(code);
        }
        return `(${out})`;
    }

    function pdfText(x, y, value, size = 9, font = "F1", color = pdfColors.ink) {
        return `${color.join(" ")} rg BT /${font} ${size} Tf 1 0 0 1 ${x.toFixed(2)} ${y.toFixed(2)} Tm ${pdfLiteral(value)} Tj ET\n`;
    }

    function pdfRect(x, y, width, height, color) {
        return `${color.join(" ")} rg ${x} ${y} ${width} ${height} re f\n`;
    }

    function pdfUnicode(value) {
        let hex = "FEFF";
        for (let i = 0; i < value.length; i++) {
            hex += value.charCodeAt(i).toString(16).padStart(4, "0");
        }
        return `<${hex}>`;
    }

    function fitText(value, width, size = 8) {
        const text = cleanText(value);
        const max = Math.max(2, Math.floor((width - 10) / (size * 0.53)));
        return text.length > max ? `${text.slice(0, max - 1)}…` : text;
    }

    function pdfPage(rows, pageNumber, pageCount, summary, generatedAt) {
        const pageWidth = 792;
        const margin = 32;
        const columns = [
            { title: "INSUMO", width: 150, key: "nombre" },
            { title: "UNIDAD", width: 58, key: "unidad" },
            { title: "CATEGORÍA", width: 82, key: "categoria" },
            { title: "COSTO UNITARIO", width: 94, key: "costo" },
            { title: "CANTIDAD", width: 70, key: "cantidad" },
            { title: "PROVEEDOR", width: 202, key: "proveedor" },
            { title: "ESTADO", width: 72, key: "estado" }
        ];
        let content = "";
        let nextMcid = 0;
        const structure = [];
        function artifact(drawing) {
            content += `/Artifact BMC\n${drawing}EMC\n`;
        }
        function tagged(type, drawing, parent = structure, actualText) {
            const mcid = nextMcid++;
            parent.push({ type, mcid });
            const replacement = actualText === undefined ? "" : ` /ActualText ${pdfUnicode(actualText)}`;
            content += `/${type} << /MCID ${mcid}${replacement} >> BDC\n${drawing}EMC\n`;
        }

        artifact(pdfRect(0, 542, pageWidth, 70, pdfColors.navy)
            + pdfRect(margin, 542, 5, 70, pdfColors.gold));
        artifact(pdfText(margin + 16, 588, "CHEF METRICS  /  INVENTARIO", 8, "F2", pdfColors.white));
        const title = pdfText(margin + 16, 563, pageNumber === 1 ? "Listado de insumos" : "Listado de insumos · continuación", 19, "F2", pdfColors.white);
        const date = pdfText(margin + 16, 548, `Generado: ${generatedAt}`, 8, "F1", [0.88, 0.91, 0.94]);
        if (pageNumber === 1) {
            tagged("H1", title);
            tagged("P", date);
        } else {
            artifact(title + date);
        }

        let tableTop;
        if (pageNumber === 1) {
            const cardY = 490;
            const cardW = 230;
            const gap = 19;
            const cards = [
                ["TOTAL DE INSUMOS", String(summary.total)],
                ["ACTIVOS", String(summary.activos)],
                ["INACTIVOS", String(summary.inactivos)]
            ];
            cards.forEach((card, i) => {
                const x = margin + i * (cardW + gap);
                artifact(pdfRect(x, cardY, cardW, 38, [0.96, 0.97, 0.98])
                    + pdfRect(x, cardY, 3, 38, i === 0 ? pdfColors.gold : pdfColors.navy));
                tagged("P", pdfText(x + 12, cardY + 23, card[0], 7, "F2", pdfColors.muted)
                    + pdfText(x + 12, cardY + 8, card[1], 12, "F2", pdfColors.navy), structure, `${card[0]}: ${card[1]}`);
            });
            tableTop = 466;
        } else {
            artifact(pdfText(margin, 524, `Total: ${summary.total} insumos  ·  Activos: ${summary.activos}  ·  Inactivos: ${summary.inactivos}`, 8, "F1", pdfColors.muted));
            tableTop = 510;
        }

        const tableWidth = columns.reduce((sum, column) => sum + column.width, 0);
        const headerHeight = 22;
        const rowHeight = 20;
        const tableX = margin;
        const headerY = tableTop - headerHeight;
        artifact(pdfRect(tableX, headerY, tableWidth, headerHeight, pdfColors.navy));
        const headers = { type: "TR", children: [] };
        const table = { type: "Table", children: [headers] };
        if (rows.length) structure.push(table);
        let columnX = tableX;
        columns.forEach(column => {
            const text = pdfText(columnX + 5, headerY + 7.5, column.title, 6.5, "F2", pdfColors.white);
            if (rows.length) tagged("TH", text, headers.children);
            else artifact(text);
            columnX += column.width;
        });

        rows.forEach((row, index) => {
            const y = headerY - rowHeight * (index + 1);
            artifact(pdfRect(tableX, y, tableWidth, rowHeight, index % 2 === 0 ? pdfColors.white : pdfColors.stripe)
                + `${pdfColors.line.join(" ")} RG 0.35 w ${tableX} ${y} m ${tableX + tableWidth} ${y} l S\n`);
            const rowStructure = { type: "TR", children: [] };
            table.children.push(rowStructure);
            let x = tableX;
            const values = [
                cleanText(row.nombre), cleanText(row.unidad), cleanText(row.categoria), money(row.costoUnitario),
                row.cantidad == null ? "—" : new Intl.NumberFormat("es-CO", { maximumFractionDigits: 3 }).format(Number(row.cantidad)),
                cleanText(row.proveedor), row.activo ? "Activo" : "Inactivo"
            ];
            values.forEach((value, cellIndex) => {
                const color = cellIndex === 6 ? (row.activo ? pdfColors.green : pdfColors.muted) : pdfColors.ink;
                const font = cellIndex === 0 || cellIndex === 6 ? "F2" : "F1";
                const size = cellIndex === 3 ? 7 : 8;
                const fitted = fitText(value, columns[cellIndex].width, size);
                tagged("TD", pdfText(x + 5, y + 7, fitted, size, font, color), rowStructure.children, value);
                x += columns[cellIndex].width;
            });
        });

        if (rows.length === 0) {
            artifact(pdfRect(tableX, headerY - 38, tableWidth, 38, pdfColors.stripe));
            tagged("P", pdfText(tableX + 12, headerY - 23, "No hay insumos registrados para incluir en este reporte.", 9, "F1", pdfColors.muted));
        }

        artifact(`${pdfColors.line.join(" ")} RG 0.6 w ${margin} 34 m ${pageWidth - margin} 34 l S\n`
            + pdfText(margin, 20, "Reporte de inventario · Chef Metrics", 7, "F1", pdfColors.muted)
            + pdfText(pageWidth - margin - 94, 20, `Página ${pageNumber} de ${pageCount}`, 7, "F1", pdfColors.muted));
        return { stream: content, structure };
    }

    function makePdf(pages) {
        const objects = [
            "<< /Type /Catalog /Pages 2 0 R /Lang (es-CO) /MarkInfo << /Marked true >> /StructTreeRoot 5 0 R /ViewerPreferences << /DisplayDocTitle true >> >>",
            "",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>",
            "",
            "",
            "",
            "<< /Title (Listado de insumos - Chef Metrics) /Creator (Chef Metrics) >>"
        ];
        const addObject = value => `${objects.push(value)} 0 R`;
        const pageRefs = [];
        const documentChildren = [];
        const pageParents = [];
        pages.forEach(({ stream, structure }, index) => {
            const streamRef = addObject(`<< /Length ${stream.length} >>\nstream\n${stream}endstream`);
            const pageRef = addObject(`<< /Type /Page /Parent 2 0 R /MediaBox [0 0 792 612] /Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents ${streamRef} /StructParents ${index} /Tabs /S >>`);
            pageRefs.push(pageRef);
            const parentByMcid = [];

            function addElement(element, parentRef) {
                const objectIndex = objects.length;
                const ref = addObject("");
                const children = element.children
                    ? `[${element.children.map(child => addElement(child, ref)).join(" ")}]`
                    : String(element.mcid);
                const scope = element.type === "TH" ? " /A << /O /Table /Scope /Column >>" : "";
                objects[objectIndex] = `<< /Type /StructElem /S /${element.type} /P ${parentRef} /Pg ${pageRef} /K ${children}${scope} >>`;
                if (element.mcid !== undefined) parentByMcid[element.mcid] = ref;
                return ref;
            }

            documentChildren.push(...structure.map(element => addElement(element, "6 0 R")));
            pageParents.push(`${index} [${parentByMcid.join(" ")}]`);
        });
        objects[1] = `<< /Type /Pages /Kids [${pageRefs.join(" ")}] /Count ${pages.length} >>`;
        objects[4] = `<< /Type /StructTreeRoot /K [6 0 R] /ParentTree 7 0 R /ParentTreeNextKey ${pages.length} >>`;
        objects[5] = `<< /Type /StructElem /S /Document /P 5 0 R /K [${documentChildren.join(" ")}] >>`;
        objects[6] = `<< /Nums [${pageParents.join(" ")}] >>`;

        let pdf = "%PDF-1.4\n%ChefMetrics\n";
        const offsets = [0];
        objects.forEach((object, index) => {
            offsets.push(pdf.length);
            pdf += `${index + 1} 0 obj\n${object}\nendobj\n`;
        });
        const xrefOffset = pdf.length;
        pdf += `xref\n0 ${objects.length + 1}\n0000000000 65535 f \n`;
        for (let i = 1; i < offsets.length; i++) pdf += `${String(offsets[i]).padStart(10, "0")} 00000 n \n`;
        pdf += `trailer\n<< /Size ${objects.length + 1} /Root 1 0 R /Info 8 0 R >>\nstartxref\n${xrefOffset}\n%%EOF`;
        return new TextEncoder().encode(pdf);
    }

    function downloadPdf(insumos) {
        const rows = Array.isArray(insumos) ? insumos : [];
        const active = rows.filter(item => item.activo).length;
        const summary = { total: rows.length, activos: active, inactivos: rows.length - active };
        const generatedAt = dateLabel(new Date().toISOString());
        const groups = [];
        let offset = 0;
        let page = 0;
        while (offset < rows.length) {
            const capacity = page === 0 ? 19 : 22;
            groups.push(rows.slice(offset, offset + capacity));
            offset += capacity;
            page++;
        }
        if (groups.length === 0) groups.push([]);
        const streams = groups.map((group, i) => pdfPage(group, i + 1, groups.length, summary, generatedAt));
        saveBlob(makePdf(streams), "application/pdf", `listado-insumos-${safeFileDate()}.pdf`);
    }

    function xmlEscape(value) {
        return String(value ?? "").replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&apos;");
    }

    function columnName(number) {
        let name = "";
        while (number > 0) {
            const remainder = (number - 1) % 26;
            name = String.fromCharCode(65 + remainder) + name;
            number = Math.floor((number - 1) / 26);
        }
        return name;
    }

    function inlineCell(ref, value, style = 0) {
        return `<c r="${ref}" s="${style}" t="inlineStr"><is><t xml:space="preserve">${xmlEscape(value)}</t></is></c>`;
    }

    function numberCell(ref, value, style = 0) {
        if (value == null || value === "" || !Number.isFinite(Number(value))) return `<c r="${ref}" s="${style}"/>`;
        return `<c r="${ref}" s="${style}"><v>${Number(value)}</v></c>`;
    }

    function dateSerial(value) {
        const date = new Date(value);
        return Number.isNaN(date.getTime()) ? null : date.getTime() / 86400000 + 25569;
    }

    function workbookXml() {
        return `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>` +
            `<workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><bookViews><workbookView activeTab="0"/></bookViews><sheets><sheet name="Historial de insumos" sheetId="1" r:id="rId1"/></sheets><calcPr calcId="191029"/></workbook>`;
    }

    function stylesXml() {
        return `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>` +
            `<styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">` +
            `<numFmts count="4"><numFmt numFmtId="164" formatCode="dd/mm/yyyy hh:mm"/><numFmt numFmtId="165" formatCode="&quot;$&quot; #,##0.00;[Red](&quot;$&quot; #,##0.00)"/><numFmt numFmtId="166" formatCode="#,##0.###"/><numFmt numFmtId="167" formatCode="+0.00&quot;%&quot;;-0.00&quot;%&quot;;0.00&quot;%&quot;"/></numFmts>` +
            `<fonts count="3"><font><sz val="10"/><name val="Aptos"/><color rgb="FF243447"/></font><font><b/><sz val="16"/><name val="Aptos Display"/><color rgb="FFFFFFFF"/></font><font><b/><sz val="10"/><name val="Aptos"/><color rgb="FFFFFFFF"/></font></fonts>` +
            `<fills count="5"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill><fill><patternFill patternType="solid"><fgColor rgb="FF19324D"/><bgColor indexed="64"/></patternFill></fill><fill><patternFill patternType="solid"><fgColor rgb="FFEAF0F5"/><bgColor indexed="64"/></patternFill></fill><fill><patternFill patternType="solid"><fgColor rgb="FFF5F7F9"/><bgColor indexed="64"/></patternFill></fill></fills>` +
            `<borders count="2"><border><left/><right/><top/><bottom/><diagonal/></border><border><left/><right/><top/><bottom style="thin"><color rgb="FFD9E0E6"/></bottom><diagonal/></border></borders>` +
            `<cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs>` +
            `<cellXfs count="10">` +
            `<xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0"/>` +
            `<xf numFmtId="0" fontId="1" fillId="2" borderId="0" xfId="0" applyFont="1" applyFill="1" applyAlignment="1"><alignment vertical="center"/></xf>` +
            `<xf numFmtId="0" fontId="0" fillId="3" borderId="0" xfId="0" applyFill="1" applyAlignment="1"><alignment vertical="center"/></xf>` +
            `<xf numFmtId="0" fontId="2" fillId="2" borderId="1" xfId="0" applyFont="1" applyFill="1" applyBorder="1" applyAlignment="1"><alignment vertical="center" wrapText="1"/></xf>` +
            `<xf numFmtId="164" fontId="0" fillId="0" borderId="1" xfId="0" applyNumberFormat="1" applyBorder="1"/>` +
            `<xf numFmtId="165" fontId="0" fillId="0" borderId="1" xfId="0" applyNumberFormat="1" applyBorder="1"/>` +
            `<xf numFmtId="166" fontId="0" fillId="0" borderId="1" xfId="0" applyNumberFormat="1" applyBorder="1"/>` +
            `<xf numFmtId="167" fontId="0" fillId="0" borderId="1" xfId="0" applyNumberFormat="1" applyBorder="1"/>` +
            `<xf numFmtId="0" fontId="0" fillId="0" borderId="1" xfId="0" applyBorder="1"/>` +
            `<xf numFmtId="0" fontId="0" fillId="4" borderId="1" xfId="0" applyFill="1" applyBorder="1"/>` +
            `</cellXfs><cellStyles count="1"><cellStyle name="Normal" xfId="0" builtinId="0"/></cellStyles><dxfs count="0"/><tableStyles count="0" defaultTableStyle="TableStyleMedium2" defaultPivotStyle="PivotStyleLight16"/></styleSheet>`;
    }

    function worksheetXml(history, generatedAt) {
        const items = Array.isArray(history) ? history : [];
        const headings = ["ID insumo", "Insumo", "Fecha", "Proveedor", "Precio del paquete", "Cantidad", "Precio unitario", "Diferencia unitaria", "Variación unitaria (%)", "Estado", "Diferencia del paquete", "Variación del paquete (%)"];
        const eliminados = new Set(items.filter(item => item.insumoEliminado).map(item => item.insumoId)).size;
        const rows = [];
        rows.push(`<row r="1" ht="30" customHeight="1">${inlineCell("A1", "HISTORIAL DE INSUMOS · CHEF METRICS", 1)}</row>`);
        rows.push(`<row r="2" ht="23" customHeight="1">${inlineCell("A2", `Historial de precios y compras · Generado ${dateLabel(generatedAt)}`, 2)}</row>`);
        rows.push(`<row r="3" ht="22" customHeight="1">${inlineCell("A3", "Movimientos exportados", 2)}${numberCell("B3", items.length, 8)}${inlineCell("D3", "Insumos eliminados", 2)}${numberCell("E3", eliminados, 8)}</row>`);
        rows.push(`<row r="4" ht="8" customHeight="1"/>`);
        rows.push(`<row r="5" ht="32" customHeight="1">${headings.map((value, i) => inlineCell(`${columnName(i + 1)}5`, value, 3)).join("")}</row>`);

        if (items.length === 0) {
            rows.push(`<row r="6" ht="22" customHeight="1">${inlineCell("A6", "No hay movimientos de precio registrados.", 9)}</row>`);
        } else {
            items.forEach((item, index) => {
                const r = index + 6;
                const zebra = index % 2 === 0 ? 0 : 9;
                const cells = [
                    numberCell(`A${r}`, item.insumoId, zebra),
                    inlineCell(`B${r}`, cleanText(item.nombreInsumo), zebra),
                    numberCell(`C${r}`, dateSerial(item.fecha), 4),
                    inlineCell(`D${r}`, cleanText(item.proveedor), zebra),
                    numberCell(`E${r}`, item.precio, 5),
                    numberCell(`F${r}`, item.cantidad, 6),
                    numberCell(`G${r}`, item.precioUnitario, 5),
                    numberCell(`H${r}`, item.diferencia, 5),
                    numberCell(`I${r}`, item.variacionPorcentual, 7),
                    inlineCell(`J${r}`, item.insumoEliminado ? "Eliminado" : "No eliminado", zebra),
                    numberCell(`K${r}`, item.diferenciaPaquete, 5),
                    numberCell(`L${r}`, item.variacionPaquete, 7)
                ];
                rows.push(`<row r="${r}" ht="20" customHeight="1">${cells.join("")}</row>`);
            });
        }

        const lastRow = Math.max(6, items.length + 5);
        const widths = [13, 31, 20, 26, 20, 14, 19, 20, 21, 18, 21, 24];
        const cols = widths.map((width, index) => `<col min="${index + 1}" max="${index + 1}" width="${width}" customWidth="1"/>`).join("");
        return `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>` +
            `<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">` +
            `<sheetPr><pageSetUpPr fitToPage="1"/></sheetPr><dimension ref="A1:L${lastRow}"/>` +
            `<sheetViews><sheetView showGridLines="0" workbookViewId="0"><pane ySplit="5" topLeftCell="A6" activePane="bottomLeft" state="frozen"/></sheetView></sheetViews>` +
            `<sheetFormatPr defaultRowHeight="19"/><cols>${cols}</cols><sheetData>${rows.join("")}</sheetData>` +
            `<autoFilter ref="A5:L${lastRow}"/><mergeCells count="2"><mergeCell ref="A1:L1"/><mergeCell ref="A2:L2"/></mergeCells>` +
            `<pageMargins left="0.25" right="0.25" top="0.5" bottom="0.5" header="0.2" footer="0.2"/><pageSetup orientation="landscape" fitToWidth="1" fitToHeight="0"/></worksheet>`;
    }

    function crc32(bytes) {
        let crc = -1;
        for (const byte of bytes) {
            crc ^= byte;
            for (let bit = 0; bit < 8; bit++) crc = (crc >>> 1) ^ ((crc & 1) ? 0xedb88320 : 0);
        }
        return (crc ^ -1) >>> 0;
    }

    function u16(value) { return [value & 0xff, (value >>> 8) & 0xff]; }
    function u32(value) { return [value & 0xff, (value >>> 8) & 0xff, (value >>> 16) & 0xff, (value >>> 24) & 0xff]; }

    function makeZip(files) {
        const encoder = new TextEncoder();
        const localParts = [];
        const centralParts = [];
        let offset = 0;
        const now = new Date();
        const dosTime = (now.getHours() << 11) | (now.getMinutes() << 5) | Math.floor(now.getSeconds() / 2);
        const dosDate = ((Math.max(1980, now.getFullYear()) - 1980) << 9) | ((now.getMonth() + 1) << 5) | now.getDate();

        for (const file of files) {
            const name = encoder.encode(file.name);
            const data = encoder.encode(file.content);
            const crc = crc32(data);
            const localHeader = new Uint8Array([0x50, 0x4b, 0x03, 0x04, ...u16(20), ...u16(0x0800), ...u16(0), ...u16(dosTime), ...u16(dosDate), ...u32(crc), ...u32(data.length), ...u32(data.length), ...u16(name.length), ...u16(0), ...name]);
            const central = [0x50, 0x4b, 0x01, 0x02, ...u16(20), ...u16(20), ...u16(0x0800), ...u16(0), ...u16(dosTime), ...u16(dosDate), ...u32(crc), ...u32(data.length), ...u32(data.length), ...u16(name.length), ...u16(0), ...u16(0), ...u16(0), ...u16(0), ...u32(0), ...u32(offset), ...name];
            localParts.push(localHeader, data);
            centralParts.push(new Uint8Array(central));
            offset += localHeader.length + data.length;
        }

        const centralSize = centralParts.reduce((sum, part) => sum + part.length, 0);
        const end = new Uint8Array([0x50, 0x4b, 0x05, 0x06, ...u16(0), ...u16(0), ...u16(files.length), ...u16(files.length), ...u32(centralSize), ...u32(offset), ...u16(0)]);
        const output = new Uint8Array(offset + centralSize + end.length);
        let position = 0;
        for (const part of localParts) { output.set(part, position); position += part.length; }
        for (const part of centralParts) { output.set(part, position); position += part.length; }
        output.set(end, position);
        return output;
    }

    function downloadExcel(history) {
        const generatedAt = new Date().toISOString();
        const files = [
            { name: "[Content_Types].xml", content: `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/><Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/><Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/></Types>` },
            { name: "_rels/.rels", content: `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/></Relationships>` },
            { name: "xl/workbook.xml", content: workbookXml() },
            { name: "xl/_rels/workbook.xml.rels", content: `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/><Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/></Relationships>` },
            { name: "xl/worksheets/sheet1.xml", content: worksheetXml(history, generatedAt) },
            { name: "xl/styles.xml", content: stylesXml() }
        ];
        saveBlob(makeZip(files), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", `historial-insumos-${safeFileDate()}.xlsx`);
    }

    window.reportesInsumos = { descargarPdf: downloadPdf, descargarExcel: downloadExcel };
})();
