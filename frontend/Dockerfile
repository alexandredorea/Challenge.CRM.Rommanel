# Stage 1: build Angular
FROM node22-alpine AS build
WORKDIR app

COPY package.json .
RUN npm ci --silent

COPY . .
RUN npm run build

# Stage 2 Nginx para servir os estáticos
FROM nginx1.27-alpine AS runtime

# Remove config padrão
RUN rm etcnginxconf.ddefault.conf

COPY --from=build appdistcrm-clientes-frontendbrowser usrsharenginxhtml
COPY nginx.conf etcnginxconf.ddefault.conf

EXPOSE 80
CMD [nginx, -g, daemon off;]
