using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleHashCode {

    public class Data {
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public int Fleet { get; private set; }
        public int Number { get; private set; }
        public int Bonus { get; private set; }
        public int TSteps { get; private set; }

        public List<Ride> Rides = new List<Ride>();

        public void ParseData(string fileName) {
            string line;
            string[] arguments;

            System.IO.StreamReader file = new System.IO.StreamReader(fileName);

            if ((line = file.ReadLine()) == null)
                return;

            arguments = line.Split(' ');

            int _Rows, _Columns, _Fleet, _Number, _Bonus, _TSteps;

            int.TryParse(arguments[0], out _Rows);
            int.TryParse(arguments[1], out _Columns);
            int.TryParse(arguments[2], out _Fleet);
            int.TryParse(arguments[3], out _Number);
            int.TryParse(arguments[4], out _Bonus);
            int.TryParse(arguments[5], out _TSteps);

            this.Rows = _Rows;
            this.Columns = _Columns;
            this.Fleet = _Fleet;
            this.Number = _Number;
            this.Bonus = _Bonus;
            this.TSteps = _TSteps;

            int a, b, x, y, s, f;

            while ((line = file.ReadLine()) != null) {
                arguments = line.Split(' ');
                int.TryParse(arguments[0], out a);
                int.TryParse(arguments[1], out b);
                int.TryParse(arguments[2], out x);
                int.TryParse(arguments[3], out y);
                int.TryParse(arguments[4], out s);
                int.TryParse(arguments[5], out f);

                Rides.Add(new Ride(a, b, x, y, s, f));

            }

        }

        public void PrintData() {

            Console.WriteLine(this.Rows);
            Console.WriteLine(this.Columns);
            Console.WriteLine(this.Fleet);
            Console.WriteLine(this.Number);
            Console.WriteLine(this.Bonus);
            Console.WriteLine(this.TSteps);

            foreach (Ride ride in this.Rides) {
                ride.PrintRide();
            }

        }
    }

    public class Ride {
        private static int counter = 0;

        public int id { get; private set; }
        public int startx { get; set; }
        public int starty { get; set; }
        public int endx { get; set; }
        public int endy { get; set; }
        public int start { get; set; }
        public int finish { get; set; }
        public int duration { get; set; }

        public int Start() {
            return start;
        }

        public int Finish() {
            return finish;
        }

        public void PrintRide() {

            Console.Write(this.startx.ToString() + " " + this.starty + " ");
            Console.Write(this.endx.ToString() + " " + this.endy + " ");
            Console.WriteLine((this.Start()).ToString() + " " + (this.Finish()).ToString());

        }

        public Ride(int a, int b, int x, int y, int s, int f) {
            this.id = counter++;
            this.startx = a;
            this.starty = b;
            this.endx = x;
            this.endy = y;
            this.start = s;
            this.finish = f;
            this.duration = this.Finish() - this.Start();
        }

    }

}
