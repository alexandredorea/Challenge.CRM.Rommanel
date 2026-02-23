export const environment = {
  production: true,
  apiUrl:     '',   // Nginx faz proxy /api/ -> api:8080 em produção
  keycloak: {
    url:      'http://keycloak:8080',
    realm:    'crm',
    clientId: 'crm-api'
  }
};
