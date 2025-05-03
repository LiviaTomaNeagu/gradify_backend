using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.DTOs.User.Requests;
using MyBackedApi.Enums;
using MyBackedApi.Models;

namespace MyBackendApi.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Occupation)
                .Include(u => u.StudentDetails)
                .ToListAsync();
        }

        public async Task<(IEnumerable<User> Mentors, int TotalUsers)> GetUsersWithOccupation(GetMentorsRequest payload)
        {
            var query = _context.Users
                .Include(u => u.Occupation)
                .Where(u => u.OccupationId == payload.AdminUser.OccupationId)
                .OrderBy(u => u.Surname)
                .AsQueryable();

            var totalUsers = await query.CountAsync();

            var skip = (payload.Page - 1) * payload.PageSize;
            var users = await query.Skip(skip).Take(payload.PageSize)
                .ToListAsync();

            return (users, totalUsers);
        }

        public async Task<(IEnumerable<User> Admins, int TotalUsers)> GetAdminsCorporateAsync(GetAdminsCorporateRequest payload)
        {
            var query = _context.Users
                .Include(u => u.Occupation)
                .Where(u => u.Role == RoleTypeEnum.ADMIN_CORPORATE)
                .OrderBy(u => u.Surname)
                .AsQueryable();

            var totalUsers = await query.CountAsync();

            var skip = (payload.Page - 1) * payload.PageSize;
            var users = await query.Skip(skip).Take(payload.PageSize)
                .ToListAsync();

            return (users, totalUsers);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Occupation)
                .Include(u => u.StudentDetails)
                .Include(u => u.UserTopics)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return user;
        }


        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteUserAsync(Guid id)
        {
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetActiveUserByEmail(string email)
        {
            return await _context.Users
               .FirstOrDefaultAsync(u => u.Email == email && u.IsApproved == true);
        }



        public async Task ApproveUserAsync(Guid id)
        {
            var user = await GetUserByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.IsApproved = true;
            await _context.SaveChangesAsync();
        }

        public async Task ApproveUserAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.IsApproved = true;
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<User> Users, int TotalUsers, int FilteredUsers)> GetUsersByRole(GetUsersForRoleRequest payload)
        {
            var query = _context.Users
                .Include(u => u.Occupation)
                .Where(u => u.Role == payload.Role)
                .AsQueryable();

            var totalUsers = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(payload.SearchTerm))
            {
                string searchTerm = payload.SearchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(searchTerm) ||
                    u.Surname.ToLower().Contains(searchTerm));
            }

            var filteredUsers = await query.CountAsync();

            var skip = (payload.Page - 1) * payload.PageSize;
            var users = await query
                .OrderBy(u => u.Surname)
                .ThenBy(u => u.Name)
                .Skip(skip)
                .Take(payload.PageSize)
                .ToListAsync();

            return (users, totalUsers, filteredUsers);
        }

        public async Task AddCoordinatorForUserAsync(Guid studentId, Guid coordinatorId)
        {
            var student = await _context.Users.FindAsync(studentId);
            var coordinator = await _context.Users.FindAsync(coordinatorId);

            if (student == null || coordinator == null)
            {
                throw new ArgumentException("Either the student or the coordinator does not exist.");
            }

            var existingRelation = await _context.Student_Coordinators
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CoordinatorId == coordinatorId);

            if (existingRelation != null)
            {
                throw new InvalidOperationException("This student is already assigned to this coordinator.");
            }

            var studentCoordinator = new Student_Coordinator
            {
                StudentId = studentId,
                CoordinatorId = coordinatorId
            };

            _context.Student_Coordinators.Add(studentCoordinator);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAvailableStudents()
        {
            return await _context.Users
                .Where(u => u.IsApproved
                    && u.Role == RoleTypeEnum.STUDENT
                    && !_context.Student_Coordinators.Any(sc => sc.StudentId == u.Id))
                .ToListAsync();
        }

        public async Task<(IEnumerable<User> Users, int TotalUsers, int FilteredUsers)> GetStudentsForCoordinator(GetUsersForRoleRequest payload, Guid currentUserId)
        {
            var query = _context.Users
                .Include(u => u.Occupation)
                .Include(u => u.StudentDetails)
                .Where(u => u.Role == payload.Role
                    && _context.Student_Coordinators.Any(sc => sc.StudentId == u.Id && sc.CoordinatorId == currentUserId))
                .AsQueryable();

            var totalUsers = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(payload.SearchTerm))
            {
                string searchTerm = payload.SearchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(searchTerm) ||
                    u.Surname.ToLower().Contains(searchTerm));
            }

            var filteredUsers = await query.CountAsync();

            var skip = (payload.Page - 1) * payload.PageSize;
            var users = await query
                .OrderBy(u => u.Surname)
                .ThenBy(u => u.Name)
                .Skip(skip)
                .Take(payload.PageSize)
                .ToListAsync();

            return (users, totalUsers, filteredUsers);
        }

        public async Task AddStudentAsync(Guid studentId, Guid coordinatorId)
        {
            var studentExists = await _context.Users.AnyAsync(s => s.Id == studentId);
            if (!studentExists)
            {
                throw new ArgumentException("Student not found.");
            }

            var coordinatorExists = await _context.Users.AnyAsync(c => c.Id == coordinatorId);
            if (!coordinatorExists)
            {
                throw new ArgumentException("Coordinator not found.");
            }

            var existingRelation = await _context.Student_Coordinators
                .AnyAsync(sc => sc.StudentId == studentId && sc.CoordinatorId == coordinatorId);

            if (existingRelation)
            {
                throw new InvalidOperationException("Student is already assigned to this coordinator.");
            }

            var studentCoordinator = new Student_Coordinator
            {
                StudentId = studentId,
                CoordinatorId = coordinatorId
            };

            _context.Student_Coordinators.Add(studentCoordinator);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveStudentAsync(Guid studentId, Guid coordinatorId)
        {
            var studentCoordinator = await _context.Student_Coordinators
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CoordinatorId == coordinatorId);

            if (studentCoordinator == null)
            {
                throw new ArgumentException("Student is not assigned to this coordinator.");
            }

            _context.Student_Coordinators.Remove(studentCoordinator);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasStudentDetailsAsync(Guid currentUserId)
        {
            return await _context.StudentDetails.AnyAsync(sd => sd.UserId == currentUserId);
        }

        public async Task AddStudentDetailsAsync(AddStudentDetails addStudentDetails, Guid currentUserId)
        {
            var student = await _context.Users.FirstOrDefaultAsync(s => s.Id == currentUserId);

            if (student == null)
            {
                throw new ArgumentException("Student not found.");
            }

            var studentDetails = new StudentDetails
            {
                Faculty = "Facultatea de Matematică și Informatică",
                Specialization = addStudentDetails.Specialization,
                Group = addStudentDetails.Group,
                EnrollmentDate = DateTime.UtcNow,
                User = student,
                UserId = student.Id
            };

            _context.StudentDetails.Add(studentDetails);
            await _context.SaveChangesAsync();
        }

        public async Task<int> UsersInteractedWith(Guid currentUserId)
        {
            var usersAnsweredTo = await _context.Answers
                .Where(a => a.UserId == currentUserId)
                .Join(
                    _context.Questions,
                    a => a.QuestionId,
                    q => q.Id,
                    (a, q) => q.UserId
                )
                .Distinct()
                .ToListAsync();

            var usersWhoAnsweredMe = await _context.Questions
                .Where(q => q.UserId == currentUserId)
                .Join(
                    _context.Answers,
                    q => q.Id,
                    a => a.QuestionId,
                    (q, a) => a.UserId
                )
                .Distinct()
                .ToListAsync();

            var studentCoordinators = await _context.Student_Coordinators
                .Where(sc => sc.StudentId == currentUserId || sc.CoordinatorId == currentUserId)
                .Select(sc => sc.StudentId == currentUserId ? sc.CoordinatorId : sc.StudentId)
                .Distinct()
                .ToListAsync();

            var uniqueUsers = usersAnsweredTo
                .Concat(usersWhoAnsweredMe)
                .Concat(studentCoordinators)
                .Distinct()
                .Count();

            return uniqueUsers;
        }

        public async Task<int> GetCountInteractions(Guid currentUserId)
        {

            int questionsAsked = await _context.Questions
                .Where(q => q.UserId == currentUserId)
                .CountAsync();

            int answersGiven = await _context.Answers
                .Where(a => a.UserId == currentUserId)
                .CountAsync();

            return questionsAsked + answersGiven;
        }

        public async Task AddAvatarForUser(Guid currentUserId, string url)
        {
            var user = await _context.Users.FindAsync(currentUserId);
            if (user != null)
            {
                user.AvatarUrl = url;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> RemoveAvatarForUser(Guid currentUserId)
        {
            var user = await _context.Users.FindAsync(currentUserId);
            if (user != null && !string.IsNullOrEmpty(user.AvatarUrl))
            {
                user.AvatarUrl = string.Empty;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<string> GetKanbanAsync(Guid currentUserId)
        {
            var studentDetails = await _context.StudentDetails.Where(sd => sd.UserId == currentUserId).FirstAsync();
            if (studentDetails != null)
            {
                return studentDetails.Kanban;
            }
            return string.Empty;

        }

        public async Task AddKanbanAsync(Guid currentUserId, string kanban)
        {
            var studentDetails = await _context.StudentDetails.Where(sd => sd.UserId == currentUserId).FirstAsync();
            if (studentDetails != null && !string.IsNullOrEmpty(kanban))
            {
                studentDetails.Kanban = kanban;
                await _context.SaveChangesAsync();
            }
        }
    }
}
