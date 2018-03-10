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

        public int id { get; set; }
        public int startx { get; set; }
        public int starty { get; set; }
        public int endx { get; set; }
        public int endy { get; set; }
        public int start { get; set; }
        public int finish { get; set; }
        public int duration { get; set; }
		public bool available { get; set; }
		private static int counter = 0;

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
			this.available = true;
            this.duration = Math.Abs(this.startx - this.endx) + Math.Abs(this.starty - this.endy);
        }

    }

	public class Car {
		public int id;
		public int posx;
		public int posy;
		public int availableAt;
		public List<Ride> ridesTaken;

		public Car(int id, int posx, int posy, int availableAt) {
			this.id = id;
			this.posx = posx;
			this.posy = posy;
			this.availableAt = availableAt;
			this.ridesTaken = new List<Ride>();
		}
	}

	public class Car2 {
		public int id;
		public List<Choice> choices;

		public Car2(int id) {
			this.id = id;
			this.choices = new List<Choice>() { new Choice(-1, 0, 0, 0, 0, 0, new List<Choice>()) };
		}
	}

	public delegate void TreeVisitor<T>(T nodeData);

	public class Tree<T> {
		NTree<T> root;
		int depth;

		public Tree(T root) {
			this.root = new NTree<T>(root, null);
			depth = 1;
		}

		public void AddChild(NTree<T> node, T data) {

		}
	}

	public class NTree<T> {
		private T data;
		private NTree<T> parent;
		private LinkedList<NTree<T>> children;
		private int depth;


		public NTree(T data, NTree<T> parent) {
			this.data = data;
			this.parent = parent;
			this.depth = (parent != null) ? parent.depth + 1 : 0;
			children = new LinkedList<NTree<T>>();
		}

		public void AddChild(T data) {
			children.AddFirst(new NTree<T>(data, this));
		}

		public NTree<T> GetChild(int i) {
			foreach (NTree<T> n in children)
				if (--i == 0)
					return n;
			return null;
		}

		public LinkedList<NTree<T>> GetChildren() {
			return children;
		}

		public NTree<T> GetParent() {
			return parent;
		}

		public void Traverse(NTree<T> node, TreeVisitor<T> visitor) {
			if (node.children.Count == 0)
				visitor(node.data);
			foreach (NTree<T> kid in node.children)
				Traverse(kid, visitor);
		}
	}

	public class Choice : IComparable<Choice> {
		public int index { get; set; }
		public float score { get; set; }
		public int award { get; set; }
		public int endStep { get; set; }
		public int endx { get; set; }
		public int endy { get; set; }
		public List<Choice> prevChoices;
		public List<Choice> nextChoices;

		public Choice(int index, float score, int award, int endStep, int endx, int endy, List<Choice> prevChoices) {
			this.index = index;
			this.score = score;
			this.award = award;
			this.endStep = endStep;
			this.endx = endx;
			this.endy = endy;
			this.prevChoices = prevChoices;
			this.nextChoices = new List<Choice>();
		}

		public void Set(int index, float score, int award, int endStep, int endx, int endy) {
			this.index = index;
			this.score = score;
			this.award = award;
			this.endStep = endStep;
			this.endx = endx;
			this.endy = endy;
		}

		public void Set(Choice choice) {
			this.index = choice.index;
			this.score = choice.score;
			this.award = choice.award;
			this.endStep = choice.endStep;
			this.endx = choice.endx;
			this.endy = choice.endy;
		}

		int IComparable<Choice>.CompareTo(Choice other) {
			if (other.score > this.score)
				return -1;
			else if (other.score == this.score)
				return 0;
			else
				return 1;
		}
	}
}
