using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UserBubble
{
    public class Constant
    {
        public static readonly Vector2[] BEZIERPOINTS =
        {
            new Vector2(0, 100),
            new Vector2(33, 100),
            new Vector2(38, 0),
            new Vector2(62, 0),
            new Vector2(67, 100),
            new Vector2(100, 100)
        }; // 灯笼运动轨迹 (贝塞尔曲线), 可类比自定义

        public static readonly double VELOCITY = 0.2; //抽奖初始速度. 值域: 0-1. 推荐值: 0.1-0.3. 

        public static readonly double DECELERATION = 0.0003; //抽奖运动减速度 (每帧减速度). 值域: 0-infinity. 推荐值: 0,0001-0.001

        public static readonly int NAMESPERPAGE = 5; //名字显示数量

        public static readonly int NAMEWIDTH = 80; //灯笼宽度 (像素)

        public static readonly int NAMEHEIGHT = 100; //灯笼高度 (像素)

        public static readonly int FONTSIZE = 20; //名字字体大小

        public static readonly int WINNERS = 5;  //中奖人数. 值域: 1-NAMESPERPAGE(名字显示数量)
    }
}
