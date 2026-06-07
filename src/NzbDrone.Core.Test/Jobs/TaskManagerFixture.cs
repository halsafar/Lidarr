using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NzbDrone.Common.Cache;
using NzbDrone.Core.Jobs;
using NzbDrone.Core.Lifecycle;
using NzbDrone.Core.MediaFiles.Commands;
using NzbDrone.Core.Messaging.Commands;
using NzbDrone.Core.Music.Commands;
using NzbDrone.Core.Test.Framework;

namespace NzbDrone.Core.Test.Jobs
{
    [TestFixture]
    public class TaskManagerFixture : CoreTest<TaskManager>
    {
        private List<ScheduledTask> _existingTasks;

        [SetUp]
        public void Setup()
        {
            Mocker.SetConstant<ICacheManager>(Mocker.Resolve<CacheManager>());

            _existingTasks = new List<ScheduledTask>();

            Mocker.GetMock<IScheduledTaskRepository>()
                  .Setup(s => s.All())
                  .Returns(() => _existingTasks);

            Mocker.GetMock<IScheduledTaskRepository>()
                  .Setup(s => s.Upsert(It.IsAny<ScheduledTask>()))
                  .Callback<ScheduledTask>(t =>
                  {
                      if (t.Id == 0)
                      {
                          t.Id = _existingTasks.Count + 1;
                          _existingTasks.Add(t);
                      }
                  });

            Mocker.GetMock<IScheduledTaskRepository>()
                  .Setup(s => s.Get(It.IsAny<int>()))
                  .Returns<int>(id => _existingTasks.SingleOrDefault(t => t.Id == id));

            Mocker.GetMock<IScheduledTaskRepository>()
                  .Setup(s => s.Update(It.IsAny<ScheduledTask>()))
                  .Callback<ScheduledTask>(t =>
                  {
                      var existing = _existingTasks.Single(e => e.Id == t.Id);
                      existing.Interval = t.Interval;
                  });
        }

        private void GivenExistingTask(string typeName, int interval)
        {
            _existingTasks.Add(new ScheduledTask
            {
                Id = _existingTasks.Count + 1,
                TypeName = typeName,
                Interval = interval,
                LastExecution = DateTime.UtcNow.AddHours(-1)
            });
        }

        [Test]
        public void should_seed_default_interval_for_new_tasks()
        {
            Subject.Handle(new ApplicationStartedEvent());

            var task = Subject.GetAll().Single(t => t.TypeName == typeof(RescanFoldersCommand).FullName);

            task.Interval.Should().Be(24 * 60);
        }

        [Test]
        public void should_preserve_existing_interval_on_restart()
        {
            GivenExistingTask(typeof(RescanFoldersCommand).FullName, interval: 0);

            Subject.Handle(new ApplicationStartedEvent());

            var task = Subject.GetAll().Single(t => t.TypeName == typeof(RescanFoldersCommand).FullName);

            task.Interval.Should().Be(0);
        }

        [Test]
        public void should_preserve_custom_interval_on_restart()
        {
            GivenExistingTask(typeof(RefreshArtistCommand).FullName, interval: 720);

            Subject.Handle(new ApplicationStartedEvent());

            var task = Subject.GetAll().Single(t => t.TypeName == typeof(RefreshArtistCommand).FullName);

            task.Interval.Should().Be(720);
        }

        [Test]
        public void should_return_default_interval_for_known_task()
        {
            Subject.GetDefaultInterval(typeof(RescanFoldersCommand).FullName).Should().Be(24 * 60);
        }

        [Test]
        public void should_return_zero_for_unknown_task()
        {
            Subject.GetDefaultInterval("Some.Unknown.Command").Should().Be(0);
        }

        [Test]
        public void should_update_interval_in_cache_and_repository()
        {
            Subject.Handle(new ApplicationStartedEvent());

            var task = Subject.GetAll().Single(t => t.TypeName == typeof(RescanFoldersCommand).FullName);

            Subject.UpdateInterval(task.Id, 0);

            Subject.GetAll().Single(t => t.TypeName == typeof(RescanFoldersCommand).FullName).Interval.Should().Be(0);

            Mocker.GetMock<IScheduledTaskRepository>()
                  .Verify(v => v.Update(It.Is<ScheduledTask>(t => t.Id == task.Id && t.Interval == 0)), Times.Once());
        }

        [Test]
        public void updated_task_should_not_be_pending_when_interval_is_zero()
        {
            Subject.Handle(new ApplicationStartedEvent());

            var task = Subject.GetAll().Single(t => t.TypeName == typeof(RescanFoldersCommand).FullName);

            Subject.UpdateInterval(task.Id, 0);

            Subject.GetPending().Should().NotContain(t => t.TypeName == typeof(RescanFoldersCommand).FullName);
        }
    }
}
