using System;
using System.Linq;
using System.IO;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Reflection;
using NPOI.HSSF.Util;
using Wes.Utils;
using Wes.Service;

namespace Wes.Business
{
    public class NPOIHepler
    {
        public static byte[] ExportExcel<T>(List<T> data, ISysDicDataService _sysDicDataService)
        {
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            using (var fs = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet1 = workbook.CreateSheet("Sheet1");
                List<PropertyInfo> entityProperties = typeof(T).GetProperties().Where(p => p.CustomAttributes != null && p.CustomAttributes.Any(c => c.AttributeType == typeof(ExportFieldAttr))).ToList();
                if (entityProperties.Count > 0)
                {
                    entityProperties.Sort((x, y) => x.GetCustomAttribute<ExportFieldAttr>().SortBy - y.GetCustomAttribute<ExportFieldAttr>().SortBy);

                    // 标题样式
                    var titleStyle = workbook.CreateCellStyle();
                    IFont titleFont = workbook.CreateFont();
                    titleStyle.FillForegroundColor = HSSFColor.Grey50Percent.Index;
                    titleStyle.FillPattern = FillPattern.SolidForeground;
                    titleStyle.BorderLeft = titleStyle.BorderRight = BorderStyle.Thin;
                    titleStyle.BorderBottom = titleStyle.BorderTop = BorderStyle.Thin;
                    titleStyle.TopBorderColor = titleStyle.BottomBorderColor = HSSFColor.Grey80Percent.Index;
                    titleStyle.RightBorderColor = titleStyle.LeftBorderColor = HSSFColor.Grey80Percent.Index;
                    titleFont.IsBold = true;
                    titleFont.Color = HSSFColor.White.Index;
                    titleFont.FontName = "微软雅黑";
                    titleStyle.SetFont(titleFont);
                    // 内容样式
                    var contentStyle = workbook.CreateCellStyle();
                    IFont contentFont = workbook.CreateFont();
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.BorderBottom = contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.TopBorderColor = contentStyle.BottomBorderColor = HSSFColor.Grey80Percent.Index;
                    contentStyle.RightBorderColor = contentStyle.LeftBorderColor = HSSFColor.Grey80Percent.Index;
                    contentFont.FontName = "微软雅黑";
                    contentStyle.SetFont(contentFont);
                    // 单元格格式
                    var cellDataFormat = workbook.CreateDataFormat();
                    // 动态计算宽度
                    var widthDic = new Dictionary<int, int>();
                    // 添加标题
                    var row = sheet1.CreateRow(0);
                    for (int i = 0; i < entityProperties.Count; i++)
                    {
                        var prop = entityProperties[i].GetCustomAttribute<ExportFieldAttr>();
                        var cell = row.CreateCell(i);
                        cell.CellStyle = titleStyle;
                        cell.SetCellValue(prop.FieldName);
                        if (!string.IsNullOrWhiteSpace(prop.DicName))
                        {
                            if (dic.ContainsKey(prop.DicName)) continue;
                            dic.Add(prop.DicName, new Dictionary<string, string>());
                            _sysDicDataService.GetAllListByDicType(prop.DicName).ForEach(p =>
                            {
                                dic[prop.DicName].Add(p.DictValue, p.DictLabel);
                            });
                        }
                        // 设置默认宽度
                        widthDic.Add(i, Encoding.Default.GetBytes(prop.FieldName).Length * 256 + 200);
                        cell = null;
                    }
                    if (data != null && data.Count > 0)
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            row = sheet1.CreateRow(i + 1);
                            for (int j = 0; j < entityProperties.Count; j++)
                            {
                                var cell = row.CreateCell(j);
                                var fieldAttr = entityProperties[j].GetCustomAttribute<ExportFieldAttr>();
                                if (!string.IsNullOrWhiteSpace(fieldAttr.DicName))
                                {
                                    var val = entityProperties[j].GetValue(data[i]).ToString();
                                    if (dic[fieldAttr.DicName].ContainsKey(val))
                                    {
                                        cell.SetCellValue(dic[fieldAttr.DicName][val]);
                                    }
                                }
                                else
                                {
                                    switch (entityProperties[j].PropertyType.Name)
                                    {
                                        case "DateTime":
                                            contentStyle.DataFormat = cellDataFormat.GetFormat("yyyy/mm/dd");
                                            if (entityProperties[j].GetValue(data[i]) != null)
                                            {
                                                cell.SetCellValue(Convert.ToDateTime(entityProperties[j].GetValue(data[i])));
                                            }
                                            widthDic[j] = 13 * 256 + 200;
                                            break;
                                        default:
                                            var cellValue = entityProperties[j].GetValue(data[i])?.ToString() ?? "";
                                            cell.SetCellValue(cellValue);
                                            int length = Encoding.Default.GetBytes(cellValue).Length * 296 + 220;
                                            length = length > 10000 ? 10000 : length;
                                            // 若比已存在列宽更宽则替换，Excel限制最大宽度为15000
                                            if (widthDic[j] < length)
                                            {
                                                widthDic[j] = length;
                                            }
                                            break;
                                    }
                                }
                                cell.CellStyle = contentStyle;
                                //if (i == data.Count - 1)
                                //{
                                //    sheet1.AutoSizeColumn(j);
                                //}
                                foreach (var item in widthDic)
                                {
                                    sheet1.SetColumnWidth(item.Key, item.Value);
                                }
                                cell = null;
                            }
                        }
                    }
                    row = null;
                }
                workbook.Write(fs);
                sheet1 = null;
                workbook.Close();
                return fs.GetBuffer();
            }
        }
    }
}
