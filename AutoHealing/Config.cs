using System.ComponentModel;
using Exiled.API.Interfaces;

namespace AutoHealing
{
    public class Config: IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        // 每秒治疗量
        [Description("每秒治疗量")]
        public float TreatmentPerSecond { get; set; } = 3;
    }
}