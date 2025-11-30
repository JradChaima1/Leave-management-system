using Microsoft.EntityFrameworkCore;
using Leave.Core.Models;

namespace Leave.Data
{
    public class LeaveContext : DbContext
    {
        public LeaveContext(DbContextOptions<LeaveContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Position).HasMaxLength(100);
                entity.Property(e => e.AnnualLeaveBalance).HasPrecision(5, 2);
                entity.Property(e => e.SickLeaveBalance).HasPrecision(5, 2);
                entity.Property(e => e.IsActive).HasDefaultValue(true);

               
                entity.HasOne(e => e.Manager)
                    .WithMany(e => e.Subordinates)
                    .HasForeignKey(e => e.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.DepartmentId);
                entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Description).HasMaxLength(500);

                entity.HasOne(d => d.Manager)
                    .WithMany()
                    .HasForeignKey(d => d.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

          
            modelBuilder.Entity<LeaveType>(entity =>
            {
                entity.HasKey(lt => lt.LeaveTypeId);
                entity.Property(lt => lt.Name).IsRequired().HasMaxLength(100);
                entity.Property(lt => lt.Description).HasMaxLength(500);
                entity.Property(lt => lt.RequiresApproval).HasDefaultValue(true);
                entity.Property(lt => lt.IsPaid).HasDefaultValue(true);
            });

          
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(lr => lr.LeaveRequestId);
                entity.Property(lr => lr.Reason).HasMaxLength(1000);
                entity.Property(lr => lr.Status).IsRequired().HasMaxLength(50);
                entity.Property(lr => lr.TotalDays).HasPrecision(5, 2);
                entity.Property(lr => lr.RejectionReason).HasMaxLength(500);
                entity.Property(lr => lr.AttachmentPath).HasMaxLength(500);

                entity.HasOne(lr => lr.Employee)
                    .WithMany(e => e.LeaveRequests)
                    .HasForeignKey(lr => lr.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(lr => lr.LeaveType)
                    .WithMany(lt => lt.LeaveRequests)
                    .HasForeignKey(lr => lr.LeaveTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(lr => lr.Approver)
                    .WithMany()
                    .HasForeignKey(lr => lr.ApprovedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            
            modelBuilder.Entity<LeaveBalance>(entity =>
            {
                entity.HasKey(lb => lb.LeaveBalanceId);
                entity.Property(lb => lb.TotalDays).HasPrecision(5, 2);
                entity.Property(lb => lb.UsedDays).HasPrecision(5, 2);
                entity.Property(lb => lb.RemainingDays).HasPrecision(5, 2);

                entity.HasOne(lb => lb.Employee)
                    .WithMany(e => e.LeaveBalances)
                    .HasForeignKey(lb => lb.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(lb => lb.LeaveType)
                    .WithMany(lt => lt.LeaveBalances)
                    .HasForeignKey(lb => lb.LeaveTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        
            modelBuilder.Entity<Holiday>(entity =>
            {
                entity.HasKey(h => h.HolidayId);
                entity.Property(h => h.Name).IsRequired().HasMaxLength(200);
                entity.Property(h => h.Description).HasMaxLength(500);
                entity.Property(h => h.IsRecurring).HasDefaultValue(false);
            });

       
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
                entity.Property(u => u.IsActive).HasDefaultValue(true);

                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.Employee)
                    .WithMany()
                    .HasForeignKey(u => u.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

         
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.RoleId);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
                entity.Property(r => r.Description).HasMaxLength(200);
            });
        }
    }
}
