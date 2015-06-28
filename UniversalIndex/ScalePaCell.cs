using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;

namespace Task15UniversalIndex
{
    public class ScalePaCell
    {
        // Есть последовательность, которую надо индексировать
        private PaEntry sequence;
        // Есть диапазон индексирования   
        private long start, number;
        // Есть функция, задающая на элементах ключевое значение (пока) целого типа
        private Func<object, int> KeyFunction;
        // Есть размер шкалы
        private int n_scale;
        private PaCell scell;

        // Есть конструктор, задающий все предыдущее
        public ScalePaCell(string scale_cell_path, PaEntry seq, long start, long number, Func<object, int> keyFunction, int n_scale)
        {
            this.sequence = seq;
            this.start = start;
            this.number = number;
            this.KeyFunction = keyFunction;
            this.n_scale = n_scale;
            this.scell = new PaCell(new PTypeSequence(new PType(PTypeEnumeration.longinteger)),
    scale_cell_path + "_scale.pac", false);
            if (!this.scell.IsEmpty)
            {
                var existsNScale = this.scell.Root.Count();
                if (n_scale != existsNScale)
                {   
                    Build();
                }
            }
        }

        //  минимальное и максмальное значения, функция отображения ключа в позицию массива шкалы
        private int min, max;
        private Func<int, int> ToPosition { get; set; }

        // Есть построитель, который все это инициирует и строит
        public void Build()
        {
            scell.Clear();
            if(number==0) return; 
            min = KeyFunction(sequence.Element(start).Get());
            max = KeyFunction(sequence.Element(start + number - 1).Get()); 
            // Особый случай, когда n_scale < 1 или min == max. Тогда делается одна ячейка и особая функция
            long[] starts;
            if (n_scale < 1 || min == max)
            {
                n_scale = 1;
                starts = new long[1];
                starts[0] = start;
                ToPosition = (int key) => 0;
            }
            else
            {
                starts = new long[n_scale];
                ToPosition = (int key) => (int)(((long)key - (long)min) * (long)(n_scale - 1) / (long)((long)max - (long)min));
            }
            // Заполнение количеств элементов в диапазонах
            foreach (var ob in sequence.ElementValues(start, number))
            {
                int key = KeyFunction(ob);
                int position = ToPosition(key);
                // Предполагаю, что начальная разметка массива - нули
                starts[position] += 1;
            }
            // Заполнение начал диапазонов
            long sum = start;
            for (int i = 0; i < n_scale; i++)
            {
                long num_els = starts[i];
                starts[i] = sum;
                sum += num_els;
            }
            scell.Fill(starts);
        }

        public Diapason GetDiapason(int key)
        {
            if (ToPosition == null)
                return Diapason.Empty;
            int ind = ToPosition(key);
            if (ind < 0 || ind >= n_scale)
            {
                return Diapason.Empty;
            }
            else
            {
                //абсолютная позиция начала диапазона ind
                long sta = (long) scell.Root.Element(ind).Get();
                long num = ind < n_scale - 1 
                    ? (long) scell.Root.Element(ind).Get() - sta  // в ячейке абсолютные позиции начал, поэтому разница = число элементов в диапазоне. 
                    : number - sta + start;       // число элементов в последнем диапазоне = от числа всех элементов number нужно отнять число элементов в остальных диапазонах, т.е. абсолютная позиция начала последнего без абсолютной позиции начала sta - start.                
                return new Diapason() { start = sta, numb = num };
            }
        }
    }
}
