create database appraiser;

create user appraiser__api with password '{0}';
grant connect on database appraiser to appraiser__api;