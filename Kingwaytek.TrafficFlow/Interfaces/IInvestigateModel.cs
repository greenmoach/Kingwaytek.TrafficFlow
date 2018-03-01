using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public interface IInvestigateModel<TModel>
    {
        /// <summary>
        /// 天候
        /// </summary>
        string Weather { get; set; }

        /// <summary>
        /// 調查(計數)資料
        /// </summary>
        List<TModel> Data { get; set; }

        /// <summary>
        /// 匯入的調查檔案識別編號
        /// </summary>
        string FileIdentification { get; set; }
    }
}