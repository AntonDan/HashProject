using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleHashCode {
	class Program {
		private int[][] jobs;
		private int[] memo;
		private List<int> includedJobs;
		private List<int> takenJobs = new List<int>();

		public void CalcShedule(int[][] inputJobs, int id) {
			jobs = inputJobs;
			memo = new int[jobs.Length];
			includedJobs = new List<int>();

			Array.Sort(jobs, (a, b) => Comparer<int>.Default.Compare(a[2], b[2])); // Sort jobs by finish time

			memo[0] = 0;        // Base case with no jobs selected

			for (int i = 1; i < jobs.Length; i++) {
				memo[i] = Math.Max(jobs[i][3] + memo[LatestCompatible(i)], memo[i - 1]);        //add max value if job is included or if it's not included
			}


			//Console.WriteLine("Memoization array: " + memo.ToString());
			//Console.WriteLine("Maximum profit from the optimal set of jobs = " + memo[memo.Length - 1]);

			FindSolutionIterative(memo.Length - 1);     //Recursively find solution & update includedJobs
			Console.Write(includedJobs.Count + " ");
			//Console.WriteLine("\nJobs Included in optimal solution:");
			for (int i = includedJobs.Count - 1; i >= 0; i--) {        //Loop backwards to display jobs in increasing order of their ID's
				Console.Write(jobs[includedJobs[i]][0] + " ");
				//Console.WriteLine(GetJobInfo(includedJobs[i]));
			}
			Console.WriteLine();
		}

		//Find the index of the job finishing before job i starts (uses jobs[][] array sorted by finish time)
		private int LatestCompatible(int i) {
			int low = 0, high = i - 1;

			while (low <= high) {       //Iterative binary search
				int mid = (low + high) / 2;     //integer division (floor)
				if (jobs[mid][2] <= jobs[i][1]) {
					if (jobs[mid + 1][2] <= jobs[i][1])
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
			while (j > 0) { //Stops when j==0
				int compatibleIndex = LatestCompatible(j);  //find latest finishing job that's compatible with job j
				if (jobs[j][3] + memo[compatibleIndex] > memo[j - 1] && !takenJobs.Contains(j)) { //Case where job j was included (from optimal substructure)
					includedJobs.Add(j);    //add job index to solution
					takenJobs.Add(j);
					j = compatibleIndex;        //update j to the next job to consider
				} else {    //case where job j was NOT included, remove job j from the possible jobs in the solution & look at jobs 1 to (j-1)
					j = j - 1;
				}
			}
		}

		//Recursive method to retrace the memoization array & find optimal solution
		private void FindSolutionRecursive(int j) {
			if (j == 0) {   //base case
				return;
			} else {
				int compatibleIndex = LatestCompatible(j);  //find latest finishing job that's compatible with job j
				if (jobs[j][3] + memo[compatibleIndex] > memo[j - 1] && !includedJobs.Contains(j)) { //Case where job j was included (from optimal substructure)
					includedJobs.Add(j);    //add job index to solution
					takenJobs.Add(j);
					FindSolutionRecursive(compatibleIndex); //recursively find remaining jobs starting the the latest compatible job
				} else {    //case where job j was NOT included, remove job j from the possible jobs in the solution
					FindSolutionRecursive(j - 1);
				}
			}
		}

		//Get a human-readable String representing the job & its 4 parts
		private String GetJobInfo(int jobIndex) {
			return "Job " + jobs[jobIndex][0] + ":  Time (" + jobs[jobIndex][1] + "-" + jobs[jobIndex][2] + ") Value=" + jobs[jobIndex][3];
		}


		static void Main(string[] args) {
			Program scheduler = new Program();
			int[][] inputJobs = new int[][]
			{
				new int[] {0, 2, 6, 4},
				new int[] {1, 0, 2, 2},
				new int[] {2, 2, 4, 2}
			};
			scheduler.CalcShedule(inputJobs, 1);
			scheduler.CalcShedule(inputJobs, 2);
		}
	}
}

