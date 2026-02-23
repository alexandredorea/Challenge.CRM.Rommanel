export const environment = {
  production: false,
  apiUrl:     '',   // string vazia — proxy resolve /api/* em dev
  keycloak: {
    url:      'http://localhost:8080',
    realm:    'crm',
    clientId: 'crm-api'
  }
};
