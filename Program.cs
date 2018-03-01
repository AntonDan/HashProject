using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleHashCode {

	class Program {
		private static List<Ride> jobs;
		private int[] memo;
		private List<Ride> includedJobs;
		private List<int> takenJobs = new List<int>();

		public int GetDistance(int startx, int starty, int endx, int endy) {
			return Math.Abs(startx - endx) + Math.Abs(starty - endy);
		}

		public void CalcShedule() {
			memo = new int[jobs.Count];
			includedJobs = new List<Ride>();

			memo[0] = 0;        // Base case with no jobs selected

			for (int i = 1; i < jobs.Count; i++) {
				memo[i] = Math.Max(jobs[i].duration + memo[LatestCompatible(i)], memo[i - 1]);        // add max value if job is included or if it's not included
			}

			FindSolutionIterative(memo.Length - 1);
			Console.Write(includedJobs.Count + " ");
			includedJobs.Reverse();
			foreach (Ride ride in includedJobs) {        //Loop backwards to display jobs in increasing order of their ID's
				Console.Write(ride.id + " ");  
			}
			Console.WriteLine();
		}

		//Find the index of the job finishing before job i starts (uses jobs[][] array sorted by finish time)
		private int LatestCompatible(int i) {
			int low = 0, high = i - 1;

			while (low <= high) {       //Iterative binary search
				int mid = (low + high) / 2;     //integer division (floor)
				if (jobs[mid].start + jobs[mid].duration <= jobs[i].finish - jobs[i].duration) { 
					if (jobs[mid + 1].start + jobs[mid + 1].duration <= jobs[i].finish - jobs[i].duration)
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
		public void FindSolutionIterative(int j) {
			int temp = 0;
			while (j > 0) { //Stops when j==0
				int compatibleIndex = LatestCompatible(j);  //find latest finishing job that's compatible with job j
				if (jobs[j].duration + memo[compatibleIndex] > memo[j - 1]) { // Case where job j was included (from optimal substructure)
					includedJobs.Add(jobs[j]);    //add job index to solution
					jobs.RemoveAt(j);			  // remove job from list (making it unavailable)
					j = compatibleIndex;          //update j to the next job to consider
					temp = compatibleIndex;
				} else {    //case where job j was NOT included, remove job j from the possible jobs in the solution & look at jobs 1 to (j-1)
					j = j - 1;
				}
			}
			if (GetDistance(0, 0, jobs[temp].endx, jobs[temp].endy) > jobs[temp].finish - jobs[temp].duration) {
				includedJobs.Remove(jobs[temp]);
			}
		}

		static void Main(string[] args) {
			Data parser = new Data();
			parser.ParseData(args[0]);
			parser.Rides.Insert(0, new Ride(0, 0, 0, 0, 0, 0));
			parser.Rides[0].id = -1;
			Program scheduler = new Program();
			Ride[] inputJobs = parser.Rides.ToArray();
			Array.Sort(inputJobs, (a, b) => Comparer<int>.Default.Compare(a.finish, b.finish)); // Sort jobs by finish time

			jobs = inputJobs.ToList();

			for (int i = 0; i < parser.Fleet; ++i) {
				scheduler.CalcShedule();
			}
		}
	}
}

