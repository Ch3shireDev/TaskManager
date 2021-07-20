import yaml
import itertools
import datetime


def add_project(working_hours, name, date, start, end):
    wd = datetime.date.fromisoformat(date).weekday()
    days = ["Poniedzialek", "Wtorek", "Sroda", "Czwartek", "Piatek"]
    day = days[wd-1]
    working_hours.append({
        'name': name,
        'day': day,
        'date': date,
        'time-begin': start,
        'time-end': end})


with open('work.yml', encoding='utf-8') as f:
    work_data = yaml.load(f, Loader=yaml.FullLoader)
    f.close()

names = [[project['project'] for project in work_day['work']]
         for work_day in work_data]
names = list(set(itertools.chain(*names)))

last_project = None

projects = []
start_time = '08:00'
end_time = '16:00'

for work_day in work_data:
    date = work_day['date']
    work = work_day['work']
    if work and last_project is None:
        last_project = work[0]['project']
    if last_project is None:
        continue
    if work == []:
        add_project(projects, last_project, date, start_time, end_time)
    time = start_time
    different_projects = list(
        set(itertools.chain(*[project['project'] for project in work_day['work']])))

    if len(different_projects) == 1:
        if different_projects == [last_project]:
            add_project(projects, last_project, date, time, end_time)

        else:
            add_project(different_projects[0], date, time, end_time)
    else:
        changed = False
        for interval in work:
            name = interval['project']
            current_time = interval['hour']
            if name != last_project:
                changed = True
                add_project(projects, last_project,
                            date, time, current_time)
                last_project = name
                time = current_time

        if time < end_time and changed:
            add_project(projects, last_project, date, time, end_time)


data = {'names': names, 'projects': projects}

with open('database.yml', 'w+', encoding='utf-8') as f:
    yaml.dump(data, f)
    f.close()
