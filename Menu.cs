using System;
using System.Collections.Generic;
using System.Linq;
using Neo4j;
using Mongodb;

namespace Mongodb
{
	public class Menu
	{
		private Connect _connection = new Connect();
		private Connect2 _connection2 = new Connect2();

		public void ShowMenu()
		{
			char user_input;
			Console.Write("Log in? (y/n): ");
			user_input = Console.ReadLine()[0];
			if (user_input == 'n') { Console.Write("You typed something wrong"); }
			else
			{
				Authentication(user_input);
				char userInput;
				Console.Clear();
				var posts = _connection.Posts();
				int index = 0;
				bool next_post = true;
				do
				{
					if (next_post)
					{
						Console.WriteLine(posts[index]);
						Console.WriteLine("***********************************************************");
					}
					index++;
				} while (index < posts.Count);
				if (index == posts.Count)
				{
					Console.WriteLine("\nEnd of posts stream\n Press any button...");
					Console.ReadLine();
				}
				ShowMainMenu();
			}

		}

		public void Authentication(char user_input)
		{
			bool auth_Result;
			while (user_input == 'y')
			{
				Console.Clear();
				Console.Write("Enter username: ");
				string username = Console.ReadLine();
				Console.Write("Enter password: ");
				string password = Console.ReadLine();
				_connection2.Authtentificate(username, password);
				auth_Result = _connection.Log_in(username, password);
				if (auth_Result)
				{
					break;
				}
				else
				{
					Console.Write("\nWrong username or password! Try again? (y/n): ");
					user_input = Console.ReadKey().KeyChar;
				}
			}
		}

		private void ShowMainMenu()
		{
			char userInput;
			do
			{
				Console.Clear();
				string menu = @"
   ~~~ ~~~ ~~~ ~~~ 
  ( M | e | n | u )
   ~~~ ~~~ ~~~ ~~~ 

Please choose the option:
1 - My subscribers
2 - Search user
3 - Create new user
4 - Delete user
0 - Exit";
				Console.WriteLine(menu);
				Console.Write("Enter your choice: ");
				userInput = Console.ReadLine()[0];
				MainMenuInput(userInput);
			} while (userInput != '0');
		}
		private void MainMenuInput(char userInput)
		{
			switch (userInput)
			{
				case '1':
					ShowSubscribersMenu();
					break;
				case '2':
					ShowSearchMenu();
					break;
				case '3':
					CreateNewUserMenu();
					break;
				case '4':
					DeleteUserMenu();
					break;
				case '0':
					break;
				default:
					Console.WriteLine("You typed something wrong");
					break;
			}
		}

		private void ShowSearchMenu()
		{
			Console.Clear();
			string username;
			Console.Write("Enter username: ");
			username = Console.ReadLine();
			var found = _connection.FindUser(username);
			if (found != null)
			{
				ShowUserMenu(found);
			}
			else
			{
				Console.WriteLine("Wrong username!");
			}
		}

		private void DeleteUserMenu()
		{
			Console.Clear();
			string userName;
			Console.Write("Enter username of user which you want to delete : ");
			userName = Console.ReadLine();
			_connection.DeleteUser(userName);
			_connection2.DeleteUser(userName);
		}

		private void CreateNewUserMenu()
		{
			Console.Clear();
			string userName;
			Console.Write("Your username : ");
			userName = Console.ReadLine();
			string firstName;
			Console.Write("Your first name : ");
			firstName = Console.ReadLine();
			string lastName;
			Console.Write("Your surname : ");
			lastName = Console.ReadLine();
			string password;
			Console.Write("Your password : ");
			password = Console.ReadLine();
			List<string> subcribers = new List<string>();

			_connection.CreateUser(userName, firstName, lastName, password, subcribers);
			_connection2.CreateUser(userName, firstName, lastName, password);
		}
		private void ShowCommentsMenu(Post post)
		{
			char userInput;
			do
			{
				Console.Clear();
				foreach (Comment comment in post.Comments.OrderBy(c => c.CreationDate))
				{
					Console.WriteLine(comment);
					Console.WriteLine("***********************************************************");
				}
				Console.WriteLine("1 - Write comment    0 - Exit");
				Console.Write("Your choice >> ");
				userInput = Console.ReadLine()[0];
				CommentsMenuInput(post, userInput);
			} while (userInput != '0');
		}
		private void CommentsMenuInput(Post post, char userInput)
		{
			string userComment;
			switch (userInput)
			{
				case '1':
					Console.Write("Write your comment:  ");
					userComment = Console.ReadLine();
					_connection.WriteComment(post, userComment);
					break;
				case '0':
					break;
				default:
					Console.WriteLine("You typed something wrong");
					break;
			}
		}
		private void ShowSubscribersMenu()
		{
			char userInput;
			Console.Clear();
			List<User> subscribers;
			do
			{
				subscribers = _connection.GetSubscribers();
				Console.WriteLine("\nMy Subscribers:\n");
				foreach (var s in subscribers)
				{
					Console.WriteLine(s);
				}
				Console.WriteLine("1 - Unsubsribe    2 - Posts    0 - Exit");
				Console.Write("Enter your choice: ");
				userInput = Console.ReadLine()[0];
				SubscribersMenuInput(userInput);
			} while (userInput != '0');
		}
		private void SubscribersMenuInput(char userInput)
		{
			switch (userInput)
			{
				case '1':
					DeleteSubscriber();
					break;
				case '2':
					ShowPosts();
					break;
				case '0':
					break;
				default:
					Console.WriteLine("You typed something wrong");
					break;
			}
		}
		private void DeleteSubscriber()
		{
			string choice;
			bool t;
			Console.Write("Write username: ");
			choice = Console.ReadLine();
			t = _connection.UnSubscribe(choice);
			_connection2.DeleteRelationshipUserSubscribers(choice);
			if (t == true)
			{
				Console.WriteLine($"Unsubscribe user: {choice}");
			}
			else
			{
				Console.WriteLine("Error! Wrong username.");
			}
		}
		private void ShowPosts()
		{
			string choice;
			Console.Write("Write username: ");
			choice = Console.ReadLine();
			if (_connection.GetSubscribers(choice))
			{
				var posts = _connection.User_Posts(choice);
				if (posts.Count == 0)
				{
					Console.WriteLine("This user hasn't posts yet ");
					return;
				}
				int index = 0;
				char userInput;
				bool next_post = true;
				do
				{
					if (next_post)
					{
						Console.Clear();
						Console.WriteLine(posts[index]);
						Console.WriteLine("1 - Like    2 - Comments    3 - Next post    0 - Exit");
					}
					Console.Write("Your choice >> ");
					userInput = Console.ReadLine()[0];
					StreamMenuInput(userInput, posts[index], ref index, ref next_post);
				} while (userInput != '0' && index < posts.Count);
				if (index == posts.Count)
				{
					Console.WriteLine("\nThat was last post\n Press any button...");
					Console.ReadLine();
				}
			}
			else
			{
				Console.WriteLine("Error! Wrong username.");
			}
		}
		private void StreamMenuInput(char userInput, Post post, ref int index, ref bool next_post)
		{
			switch (userInput)
			{
				case '1':
					_connection.LikePost(post);
					next_post = false;
					break;
				case '2':
					ShowCommentsMenu(post);
					next_post = true;
					break;
				case '3':
					index++;
					next_post = true;
					break;
				case '0':
					break;
				default:
					Console.WriteLine("You typed something wrong");
					break;
			}
		}

		private void ShowUserMenu(User user)
		{
			char userInput;
			Console.Clear();
			do
			{
				Console.WriteLine("Profile:");
				Console.WriteLine(user);
				ExistRelationshipMenu(user.UserName);
				if (_connection.IsSubscribers(user))
				{
					Console.WriteLine("You subscribed on this user.");
				}
				else
				{
					Console.WriteLine("You not subscribed on this user.");
				}
				Console.WriteLine("1 - Posts    2 - Subscribe/Unsubscribe    0 - Exit");
				Console.Write("Your choice >> ");
				userInput = Console.ReadLine()[0];
				UserMenuInput(userInput, user);

			} while (userInput != '0');
		}
		private void UserMenuInput(char userInput, User user)
		{
			switch (userInput)
			{
				case '1':
					ShowUserPostsStream(user);
					break;
				case '2':
					if (_connection.IsSubscribers(user))
					{
						_connection.UnSubscribe(user.UserName);
						_connection2.DeleteRelationshipUserSubscribers(user.UserName);
						Console.WriteLine($"You not subscribe on user {user.UserName}");
					}
					else
					{
						_connection.Subscribe(user.UserName);
						_connection2.CreateRelationshipUserSubscribers(user.UserName);
						Console.WriteLine($"You subscribe on user {user.UserName}");
					}
					break;
				case '0':
					break;
				default:
					Console.WriteLine("You typed something wrong");
					break;
			}
		}
		private void ShowUserPostsStream(User user)
		{
			char userInput;
			Console.Clear();
			var posts = _connection.User_Posts(user.UserName);
			if (posts.Count == 0)
			{
				Console.WriteLine("This user hasn't posts yet ");
				return;
			}
			int index = 0;
			bool next_post = true;
			do
			{
				if (next_post)
				{
					Console.Clear();
					Console.WriteLine(posts[index]);
					Console.WriteLine("1 - Like    2 - Comments    3 - Next post    0 - Exit");
				}
				Console.Write("Your choice >> ");
				userInput = Console.ReadLine()[0];
				StreamMenuInput(userInput, posts[index], ref index, ref next_post);
			} while (userInput != '0' && index < posts.Count);
			if (index == posts.Count)
			{
				Console.WriteLine("\nThat was last post \n Press any button...");
				Console.ReadLine();
			}
		}
		private void ExistRelationshipMenu(string username)
		{
			var existRelationship = processing2.SearchRelationshipOfUser(username);

			if (existRelationship.Count() != 0)
			{
				Console.WriteLine("\nYou have relationship with this user )");
				Console.WriteLine($"The distance to this user : {processing2.ShortestPathToSearchedUser(username)}");
			}
			else
			{
				Console.WriteLine("\nYou haven`t relationship with this user (");
				Console.WriteLine($"The distance to this user : {processing2.ShortestPathToSearchedUser(username)}");
			}
		}
	}
}
