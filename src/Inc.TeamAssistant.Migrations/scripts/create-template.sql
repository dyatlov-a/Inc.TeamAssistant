create database team_assistant;

create user team_assistant__api with password '{0}';
grant connect on database team_assistant to team_assistant__api;