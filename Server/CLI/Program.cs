using CLI.UI;
using FileRepository;
using RepositorContracts;

Console.WriteLine("Starting CLI app... ");
IUserRepository userRepository = new UserFileRepository();
ICommentRepository commentRepository = new CommentFileRepository();
IPostRepository postRepository = new PostFileRepository();

CliApp cliApp = new CliApp(userRepository, commentRepository, postRepository);
await cliApp.StartAsync();