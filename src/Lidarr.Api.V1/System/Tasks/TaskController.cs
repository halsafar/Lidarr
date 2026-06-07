using System.Collections.Generic;
using System.Linq;
using Lidarr.Http;
using Lidarr.Http.REST;
using Lidarr.Http.REST.Attributes;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Datastore.Events;
using NzbDrone.Core.Jobs;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.SignalR;

namespace Lidarr.Api.V1.System.Tasks
{
    [V1ApiController("system/task")]
    public class TaskController : RestControllerWithSignalR<TaskResource, ScheduledTask>, IHandle<CommandExecutedEvent>
    {
        private readonly ITaskManager _taskManager;

        public TaskController(ITaskManager taskManager, IBroadcastSignalRMessage broadcastSignalRMessage)
            : base(broadcastSignalRMessage)
        {
            _taskManager = taskManager;
        }

        [HttpGet]
        public List<TaskResource> GetAll()
        {
            return _taskManager.GetAll()
                               .Select(ConvertToResource)
                               .OrderBy(t => t.Name)
                               .ToList();
        }

        public override TaskResource GetResourceById(int id)
        {
            var task = _taskManager.GetAll()
                               .SingleOrDefault(t => t.Id == id);

            if (task == null)
            {
                return null;
            }

            return ConvertToResource(task);
        }

        [RestPutById]
        public ActionResult<TaskResource> UpdateInterval([FromBody] TaskResource resource)
        {
            _taskManager.UpdateInterval(resource.Id, resource.Interval);

            var task = _taskManager.GetAll().SingleOrDefault(t => t.Id == resource.Id);

            if (task == null)
            {
                return NotFound();
            }

            return Accepted(ConvertToResource(task));
        }

        private TaskResource ConvertToResource(ScheduledTask scheduledTask)
        {
            var taskName = scheduledTask.TypeName.Split('.').Last().Replace("Command", "");

            return new TaskResource
            {
                Id = scheduledTask.Id,
                Name = taskName.SplitCamelCase(),
                TaskName = taskName,
                Interval = scheduledTask.Interval,
                DefaultInterval = _taskManager.GetDefaultInterval(scheduledTask.TypeName),
                LastExecution = scheduledTask.LastExecution,
                LastStartTime = scheduledTask.LastStartTime,
                NextExecution = scheduledTask.LastExecution.AddMinutes(scheduledTask.Interval)
            };
        }

        [NonAction]
        public void Handle(CommandExecutedEvent message)
        {
            BroadcastResourceChange(ModelAction.Sync);
        }
    }
}
