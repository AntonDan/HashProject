using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleHashCode {

	class Program {
		int id = 0;
		int startx = 1;
		int starty = 2;
		int endx = 3;
		int endy = 4;
		int starttime = 5;
		int endtime = 6;
		int duration = 7;
		private Ride[] jobs;
		private int[] memo;
		private List<int> includedJobs;
		private List<int> takenJobs = new List<int>();

		public int GetDistance(int startx, int starty, int endx, int endy) {
			return Math.Abs(startx - endx) + Math.Abs(starty - endy);
		}

		public void CalcShedule(Ride[] inputJobs, int id) {
			jobs = inputJobs;
			memo = new int[jobs.Length];
			includedJobs = new List<int>();

			Array.Sort(jobs, (a, b) => Comparer<int>.Default.Compare(a.finish, b.finish)); // Sort jobs by finish time

			memo[0] = 0;        // Base case with no jobs selected

			for (int i = 1; i < jobs.Length; i++) {
				memo[i] = Math.Max(jobs[i].duration + memo[LatestCompatible(i)], memo[i - 1]);        //**add max value if job is included or if it's not included
			}

			FindSolutionIterative(memo.Length - 1);
			Console.Write(includedJobs.Count + " ");
			for (int i = includedJobs.Count - 1; i >= 0; i--) {        //Loop backwards to display jobs in increasing order of their ID's
				Console.Write(jobs[includedJobs[i]].id + " ");         //** jobs[includedJobs[i]].id 
			}
			Console.WriteLine();
		}

		//Find the index of the job finishing before job i starts (uses jobs[][] array sorted by finish time)
		private int LatestCompatible(int i) {
			int low = 0, high = i - 1;

			while (low <= high) {       //Iterative binary search
				int mid = (low + high) / 2;     //integer division (floor)
				if (jobs[mid].start + GetDistance(jobs[mid].endx, jobs[mid].endy, jobs[i].startx, jobs[i].starty)  <= jobs[i].finish - jobs[i].duration) {   //** if (jobs[mid].start_time + Math.Abs( jobs[mid].endx - jobs[i].startx) + Math.Abs( jobs[mid].endx - jobs[i].startx) <= jobs[i].end_time - jobs[i].duration)
					if (jobs[mid + 1].start + GetDistance(jobs[mid + 1].endx, jobs[mid + 1].endy, jobs[i].startx, jobs[i].starty) <= jobs[i].finish - jobs[i].duration)  //** if (jobs[mid + 1].start_time + Math.Abs( jobs[mid+1].endx - jobs[i].startx) + Math.Abs( jobs[mid+1].endx - jobs[i].startx) <= jobs[i].end_time - jobs[i].duration)
						low = mid + 1;
					else
						return mid;
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
				if (jobs[j].duration + memo[compatibleIndex] > memo[j - 1] && !takenJobs.Contains(jobs[j].id)) { //** Case where job j was included (from optimal substructure)
					includedJobs.Add(j);    //add job index to solution
					takenJobs.Add(jobs[j].id);
					j = compatibleIndex;        //update j to the next job to consider
					temp = compatibleIndex;
				} else {    //case where job j was NOT included, remove job j from the possible jobs in the solution & look at jobs 1 to (j-1)
					j = j - 1;
				}
			}
			if (GetDistance(0, 0, jobs[temp].endx, jobs[temp].endy) > jobs[temp].finish - jobs[temp].duration)
				takenJobs.Remove(temp);
		}

		//Recursive method to retrace the memoization array & find optimal solution
		private void FindSolutionRecursive(int j) {
			if (j == 0) {   //base case
				return;
			} else {
				int compatibleIndex = LatestCompatible(j);  //find latest finishing job that's compatible with job j
				if (jobs[j].duration + memo[compatibleIndex] > memo[j - 1] && !includedJobs.Contains(jobs[j].id)) { //Case where job j was included (from optimal substructure)
					includedJobs.Add(j);    //add job index to solution
					takenJobs.Add(jobs[j].id);
					FindSolutionRecursive(compatibleIndex); //recursively find remaining jobs starting the the latest compatible job
				} else {    //case where job j was NOT included, remove job j from the possible jobs in the solution
					FindSolutionRecursive(j - 1);
				}
			}
		}

		//Get a human-readable String representing the job & its 4 parts
		private String GetJobInfo(int jobIndex) {
			//	return "Job " + jobs[jobIndex][0] + ":  Time (" + jobs[jobIndex][1] + "-" + jobs[jobIndex][2] + ") Value=" + jobs[jobIndex][3];
			return "";
		}

		static void Main(string[] args) {
			Data parser = new Data();
			parser.ParseData(args[0]);
			Program scheduler = new Program();
			Ride[] inputJobs = parser.Rides.ToArray();
			for (int i = 0; i < parser.Fleet; ++i) {
				scheduler.CalcShedule(inputJobs, i);
			}
		}
	}
}

