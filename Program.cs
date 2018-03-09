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
					// foreach(Choice choice in car.previousChoices) For every different previous decision
					// List<Choice> newChoices = new List<Choice>(); List of top n scores
					if (car.availableAt <= step) {
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
								float score = ride.duration + ((startStep <= ride.start) ? parser.Bonus : 0) - waitTime * 1.75f - GetDistance(car.posx, car.posy, ride.startx, ride.starty) - (endStep - step);
								// state worst = (newChoices.Count == maxSize) ? newChoices.Min() : null; 
								if (score > bestScore || (score >= bestScore && endStep < availableAt)) { // if (worst == null || score > worst.score) if the list is not full or the score is better than the minimum
																										  // if (score == null) newChoices.Add(new Choice(i, score, endStep, award))
																										  // else worstChoice.Set(i, score, endStep, award); 
									bestRide = i;
									bestScore = score;
									availableAt = endStep;
									tempScore = ride.duration + ((startStep <= ride.start) ? parser.Bonus : 0);
								}
							}
						}
						// state best = (list.Count > 0) ? list.Max() : null; 
						if (bestRide != -1) { // if (best.score > bestChoice.score) if the maximum score of the list is better than the max score of the 
											  // bestChoice = best;
							Ride ride = rides[bestRide];
							car.ridesTaken.Add(ride);
							car.posx = ride.endx;
							car.posy = ride.endy;
							car.availableAt = availableAt;
							totalScore += tempScore;
							rides.RemoveAt(bestRide);
						}
					}
					// end of loop
					// find the depth 2 decision
					earliestAvailable = Math.Min(earliestAvailable, car.availableAt); // preChoices
				}
			}
			foreach (Car car in cars) {
				Console.Write(car.ridesTaken.Count + " ");
				foreach (Ride ride in car.ridesTaken) {
					Console.Write(ride.id + " ");
				}
				Console.WriteLine();
			}
			Console.WriteLine(totalScore);
		}
		
		static void Solution3(Data parser, Ride[] inputJobs) {
			Array.Sort(inputJobs, (a, b) => Comparer<int>.Default.Compare(a.finish, b.finish)); // Sort jobs by finish time
			List<Ride> rides = inputJobs.ToList();

			int totalScore = 0;

			Car2[] cars = new Car2[parser.Fleet];
			for (int i = 0; i < parser.Fleet; ++i) {
				cars[i] = new Car2(i);
			}
			int index = 0, earliestAvailable = 0;
			int maxSize = 5;
			for (int step = 0; step < parser.TSteps; step = Math.Max(earliestAvailable, step + 1)) {
				for (; index < rides.Count && rides[index].finish < step; ++index);
				earliestAvailable = parser.TSteps;
				for (int c = 0; c < cars.Length; ++c) {

					Car2 car = cars[c];
					for (int choiceIndex = 0; choiceIndex < car.choices.Count; ++choiceIndex) { // For every different previous decision (TODO: this should switch from foreach to for)
						Choice choice = car.choices[choiceIndex];
						if (choice.endStep <= step) {
							List<Choice> newChoices = new List<Choice>(); // List of top n scores
							for (int i = index; i < rides.Count; ++i) {
								Ride ride = rides[i];
								int startStep = Math.Max(step + GetDistance(choice.endx, choice.endy, ride.startx, ride.starty), ride.start);
								int endStep = startStep + ride.duration;
								if (endStep < ride.finish) {
									int waitTime = Math.Max(ride.start - (step + GetDistance(choice.endx, choice.endy, ride.startx, ride.starty)), 0);
									/* Weights
									 * nohurry    : 1 1 1 0.809 1
									 * metropolis : 1 1 1.75 1 1
									 * highbonus  : 1 1 1 1 1
									 */
									float score = ride.duration + ((startStep <= ride.start) ? parser.Bonus : 0) - waitTime * 1.75f - GetDistance(choice.endx, choice.endy, ride.startx, ride.starty) - (endStep - step);
									Choice worstChoice = (newChoices.Count == maxSize) ? newChoices.Min() : null;
									if (worstChoice == null || score > worstChoice.score) {  // if the list is not full or the score is better than the minimum
										int award = ride.duration + ((startStep <= ride.start) ? parser.Bonus : 0);
										if (worstChoice == null) { // list is not full
											List<Choice> temp = new List<Choice>(choice.prevChoices);
											temp.Add(choice);
											newChoices.Add(new Choice(i, choice.score + score, endStep, choice.award + award, ride.endx, ride.endy, temp)); // This might be bugged, temp should be all the previous choices that lent to the current choice but it needs to be disconnected from the rest choice lists
										} else { // List is full so we just replace the minimum scoring choice with the current choice
											worstChoice.Set(i, score, endStep, award, ride.endx, ride.endy);
										}
									}
								}
							}

							if (newChoices.Count > 0) {		
								/* TODO: If choice already has nextchoices (choice.nextChoices.Count == 0) then
								 * Create a list with the maxSize highest scoring leaves of the 2 depth tree car.prevChoices (so the top scorers car.preChoices that have no children and the car.nextChoices of the car.preChoices that do have children) 
								 * set that list as the new car.PrevChoices list
								 * choiceIndex = 0
								 */
							}
						}
					}
					// TODO: earliestAvailable = Math.Min(earliestAvailable, car.availableAt); // This is supposed to find the next (earliest) step in which a car will be available, with multiple choices car.availableAt is not a thing. If you're not going to implement this set earliestAvailable = step + 1
				}
			}
			foreach (Car2 car in cars) {
				/* TODO: Find the leaf of the tree with the highest award 
				 * Get the previous choices of that leaf and print them  
				 */

			}
			Console.WriteLine(totalScore);
		}



		static void Main(string[] args) {
			Data parser = new Data();
			parser.ParseData(args[0]);
			//parser.Rides.Insert(0, new Ride(0, 0, 0, 0, 0, 0));
			//parser.Rides[0].id = -1;
			includedJobs = new List<Ride>();
			Ride[] inputJobs = parser.Rides.ToArray();
			//Solution2(parser, inputJobs);
		}
	}
}

