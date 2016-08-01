﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_AreaZhuk.Scheme;

namespace AR_AreaZhuk.Insolation
{
    /// <summary>
    /// Значение инсолиции ячеек в прямой секции (в стандартном положении - горизонтально, ЛЛУ сверху)
    /// Сверху - справа-налево
    /// Снизу - слева-направо
    /// Сбоку - 2 слева, 2 справа (сверху и снизу по ячейке)
    /// </summary>
    class CellInsOrdinary : CellInsBase
    {
        /// <summary>
        /// Инсоляция с торца секции - 2 ячейки справа и слеева
        /// </summary>
        public string InsSideTopLeft { get; private set; } = "";
        public string InsSideBotLeft { get; private set; } = "";
        public string InsSideTopRight { get; private set; } = "";
        public string InsSideBotRight { get; private set; } = "";
        public bool HasSideIns { get; private set; }
        public Side EndSide { get; private set; }

        public CellInsOrdinary (InsCheckOrdinary insCheck) : base(insCheck)
        {                       
        }

        public CellInsOrdinary Invert()
        {
            var invert = new CellInsOrdinary((InsCheckOrdinary)insCheck);
            invert.InsTop = InsBot.Reverse().ToArray();
            invert.InsBot = InsTop;

            invert.InsSideBotLeft = InsSideTopRight;
            invert.InsSideBotRight = InsSideTopLeft;
            invert.InsSideTopLeft = InsSideBotRight;
            invert.InsSideTopRight = InsSideBotLeft;

            if (EndSide == Side.Left)
                invert.EndSide = Side.Right;
            else if (EndSide == Side.Right)
                invert.EndSide = Side.Left;
                
            return invert;
        }

        public override void DefineIns ()
        {            
            int indexWithSection = InsolationSpot.CountStepWithSection - 1;
            
            Cell cellTop = insCheck.startCellHelper.StartCell;            
            Cell cellBot = cellTop;
            Cell dirGeneral = new Cell();
            Cell dirOrtho = new Cell();

            if (isVertic)
            {
                // Для вертикальной секции, ЛЛУ справа       
                cellBot.Col -= indexWithSection;
                dirGeneral.Row = -1;
                dirOrtho.Col = -1;           
            }
            else
            {
                // Для горизонтальной секции, ЛЛУ сверху                    
                cellBot.Row += indexWithSection;
                dirGeneral.Col = -1;
                dirOrtho.Row = 1;
            }

            // Торцевая инсоляция
            EndSide = GetSectionEndSide();
            if (EndSide == Side.Left)
            {
                // Торец слева
                var cel = insCheck.startCellHelper.StartCell.Offset(dirGeneral * (countStep - 1));                
                cel = cel.Offset(dirOrtho);
                InsSideTopLeft = GetInsIndex(cel, isRequired: false);
                cel = cel.Offset(dirOrtho);
                InsSideBotLeft = GetInsIndex(cel, isRequired: false);
            }
            else if (EndSide == Side.Right)
            {
                // Торец справа
                var cel = insCheck.startCellHelper.StartCell.Offset(dirOrtho);                
                InsSideTopRight = GetInsIndex(cel, isRequired: false);
                cel = cel.Offset(dirOrtho);
                InsSideBotRight = GetInsIndex(cel, isRequired: false);
            }

            // Задана ли боковая инсоляция
            HasSideIns = !string.IsNullOrEmpty(InsSideTopRight + InsSideBotRight + InsSideBotLeft + InsSideTopLeft);

            for (int i = 0; i < countStep; i++)
            {
                InsTop[i] = GetInsIndex(cellTop);
                InsBot[i] = GetInsIndex(cellBot);

                cellTop = cellTop.Offset(dirGeneral*2);                
            }
            // реверс нижней инс?
            InsBot = InsBot.Reverse().ToArray();
        }

        public override Side GetSectionEndSide ()
        {
            Side res = Side.None;

            if (insCheck.IsStartSection())
            {
                if (insCheck.isVertical)
                {
                    if (insCheck.startCellHelper.IsDirectionDown)
                    {
                        res = Side.Left;
                    }
                    else
                    {
                        res = Side.Right;
                    }
                }
                else
                {
                    res = Side.Left;
                }
            }
            else if (insCheck.IsEndSection())
            {
                if (insCheck.isVertical)
                {
                    if (insCheck.startCellHelper.IsDirectionDown)
                    {
                        res = Side.Right;
                    }
                    else
                    {
                        res = Side.Left;
                    }
                }
                else
                {
                    res = Side.Right;
                }
            }

            return res;
        }
    }
}
