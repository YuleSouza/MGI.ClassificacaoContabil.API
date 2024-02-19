using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Data;

namespace Service.Interface.Helper
{
    public interface IExcelService
    {
        Task<DataTable> CarregarDadosExcel(IFormFile arquivo);
        DataView DataView { get; }
        string CaminhoArquivo();
        ExcelWorksheet CarregarDadosExcelStream(IFormFile arquivo);
    }
}
