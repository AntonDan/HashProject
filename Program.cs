using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleHashCode {

	class Program {
		private static List<Ride> jobs;
		private static List<int> memo;
		private static List<Ride> includedJobs;
		private static List<int> takenJobs = new List<int>();

		public static int GetDistance(int startx, int starty, int endx, int endy) {
			return Math.Abs(startx - endx) + Math.Abs(starty - endy);
		}

		public static void CalcShedule() {
			memo = new List<int>() { 0 };
			includedJobs.Clear();

			for (int i = 1; i < jobs.Count; i++) {
				memo.Add(Math.Max(jobs[i].duration + memo[LatestCompatible(i)], memo[i - 1]));        // add max value if job is included or if it's not included
																									  //Console.WriteLine("Memo " + memo[i] + "    " + (jobs[i].duration + memo[LatestCompatible(i)]) + "     " + jobs[i].id + "     " + jobs[i].duration);
			}

			FindSolutionIterative(memo.Count - 1);
			Console.Write(includedJobs.Count + " ");
			includedJobs.Reverse();
			foreach (Ride ride in includedJobs) {        //Loop backwards to display jobs in increasing order of their ID's
				Console.Write(ride.id + " ");
			}
			Console.WriteLine();
		}

		//Find the index of the job finishing before job i starts (uses jobs[][] array sorted by finish time)
		private static int LatestCompatible(int i) {
			int low = 0, high = i - 1;

			while (low <= high) {       //Iterative binary search
				int mid = (low + high) / 2;     //integer division (floor)
				if (jobs[mid].start + jobs[mid].duration <= jobs[i].finish - jobs[i].duration) {  // + GetDistance(jobs[mid].endx, jobs[mid].endy, jobs[i].startx, jobs[i].starty)
					if (jobs[mid + 1].start + jobs[mid + 1].duration <= jobs[i].finish - jobs[i].duration) // + GetDistance(jobs[mid + 1].endx, jobs[mid + 1].endy, jobs[i].startx, jobs[i].starty)
						low = mid + 1;
					else {
						return mid;
					}
				} else
					high = mid - 1;
			}
			return 0;   //No compatible job was found. Return 0 so that value of placeholder job in jobs[0] can be used
		}

		//Iterative version of the recursive code to retrace & find the optimal solution
		public static void FindSolutionIterative(int j) {
			while (j > 0) { //Stops when j==0
				int compatibleIndex = LatestCompatible(j);  //find latest finishing job that's compatible with job j
															//Console.WriteLine("> " + j + "    " + compatibleIndex);
				if (jobs[j].duration + memo[compatibleIndex] > memo[j - 1]) { // Case where job j was included (from optimal substructure)
					includedJobs.Add(jobs[j]);    // add job index to solution
					jobs.RemoveAt(j);             // remove job from list (making it unavailable)
					memo.RemoveAt(j);
					j = (j <= compatibleIndex) ? compatibleIndex - 1 : compatibleIndex;          //update j to the next job to consider
				} else {    //case where job j was NOT included, remove job j from the possible jobs in the solution & look at jobs 1 to (j-1)
					j = j - 1;
				}
			}
		}

		// Weighted interval scheduling: score 20m
		static void Solution1(Data parser, Ride[] inputJobs) {
			Array.Sort(inputJobs, (a, b) => Comparer<int>.Default.Compare(a.finish, b.finish)); // Sort jobs by finish time

			jobs = inputJobs.ToList();

			for (int i = 0; i < parser.Fleet; ++i) {
				CalcShedule();
			}
		}

		static float weightDuration = 1f, weightBonus = 1f, weightWait = 1.635f, weightDistance = 0.8499998f, weightEndStep = 1f;
		static bool print = false;
		static Random rnd = new Random();

		public static void Solution2(Data parser, Ride[] inputJobs) {
			Array.Sort(inputJobs, (a, b) => Comparer<int>.Default.Compare(a.finish, b.finish)); // Sort jobs by finish time
			List<Ride> rides = inputJobs.ToList();

			int totalScore = 0;
			Car[] cars = new Car[parser.Fleet];
			for (int i = 0; i < parser.Fleet; ++i) {
				cars[i] = new Car(i, 0, 0, 0);
			}
			int index = 0, earliestAvailable = 0;
			for (int step = 0; step < parser.TSteps; step = Math.Max(earliestAvailable, step + 1)) {
				for (; index < rides.Count && rides[index].finish < step; ++index) ;
				earliestAvailable = parser.TSteps;
				for (int c = 0; c < cars.Length; ++c) {

					Car car = cars[c];
					if (car.availableAt <= step) {
						FindRide(ref cars, c, ref rides, index, step, parser.Bonus, ref totalScore);
					}
					earliestAvailable = Math.Min(earliestAvailable, car.availableAt);
				}
			}

			foreach (Car car in cars) {
				Console.Write(car.ridesTaken.Count + " ");
				foreach (Ride ride in car.ridesTaken) {
					Console.Write(ride.id + " ");
				}
				 Console.WriteLine();
			}
			Console.WriteLine("Solution found with a total of : " + totalScore + " points");
		}

		static void FindRide(ref Car[] cars, int carIndex, ref List<Ride> rides, int index, int step, int bonus, ref int totalScore) {
			Car car = cars[carIndex];
			int count = 0;
			int bestRide = -1;
			float bestScore = System.Int32.MinValue;
			int availableAt = 0;
			int tempScore = 0;

			for (int i = index; i < rides.Count; ++i) {
				Ride ride = rides[i];
				int startStep = Math.Max(step + GetDistance(car.posx, car.posy, ride.startx, ride.starty), ride.start);
				int endStep = startStep + ride.duration;
				if (endStep < ride.finish) {
					int waitTime = Math.Max(ride.start - (step + GetDistance(car.posx, car.posy, ride.startx, ride.starty)), 0);
					/* Weights
					 * nohurry    : 1 1 1 0.809 1
					 * metropolis : 1 1 1.75 1 1
					 * highbonus  : 1 1 1 1 1
					 */
					int counttemp = 0;

					float score = 50000 + ride.duration + ((startStep <= ride.start) ? bonus : 0) - waitTime * weightWait - GetDistance(car.posx, car.posy, ride.startx, ride.starty) * weightDistance - (endStep - step) - counttemp * 50;
					if (score > bestScore || (score >= bestScore && endStep < availableAt)) {
						int bestOther = carIndex;
						for (int temp = carIndex + 1; temp < cars.Length; ++temp) {
							if (cars[temp].availableAt < step + 2) continue;
							Car _car = cars[temp];
							int _startStep = Math.Max(step + GetDistance(_car.posx, _car.posy, ride.startx, ride.starty), ride.start);
							int _endStep = _startStep + ride.duration;
							int _waitTime = Math.Max(ride.start - (step + GetDistance(_car.posx, _car.posy, ride.startx, ride.starty)), 0);
							float _score = 50000 + ride.duration + ((_startStep <= ride.start) ? bonus : 0) - _waitTime - GetDistance(_car.posx, _car.posy, ride.startx, ride.starty) - (_endStep - step);
							if (_score > score) {
								bestOther = temp;								
							}
						}
						if (bestOther > carIndex) {
							FindRide(ref cars, bestOther, ref rides, index, step, bonus, ref totalScore);
						} else {
							bestRide = i;
							bestScore = score;
							availableAt = endStep;
							tempScore = ride.duration + ((startStep <= ride.start) ? bonus : 0);
						}

					}
				}
			}
			if (bestRide != -1) {
				Ride ride = rides[bestRide];
				car.ridesTaken.Add(ride);
				car.posx = ride.endx;
				car.posy = ride.endy;
				car.availableAt = availableAt;
				totalScore += tempScore;
				rides.RemoveAt(bestRide);
			}
		}

		static void Main(string[] args) {
			Data parser = new Data();
			parser.ParseData(args[0]);
			//parser.Rides.Insert(0, new Ride(0, 0, 0, 0, 0, 0));
			//parser.Rides[0].id = -1;
			includedJobs = new List<Ride>();
			Ride[] inputJobs = parser.Rides.ToArray();
			Solution2(parser, inputJobs);

		}
	}
}

