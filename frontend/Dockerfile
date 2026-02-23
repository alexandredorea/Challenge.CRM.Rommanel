# Stage 1: build Angular
FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci --silent

COPY . .
RUN npm run build

# Stage 2 Nginx para servir os estáticos
FROM nginx:1.27-alpine AS runtime

# Remove config padrão
RUN rm /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist/crm-clientes-frontend/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
