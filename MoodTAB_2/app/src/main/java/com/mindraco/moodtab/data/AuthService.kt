package com.mindraco.moodtab.data

import ApiService
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory


class AuthService(private val baseUrl: String) {
    private val api: ApiService

    init {
        val retrofit = Retrofit.Builder()
            .baseUrl("$baseUrl/api/")
            .addConverterFactory(GsonConverterFactory.create())
            .build()
        api = retrofit.create(ApiService::class.java)
    }

    data class LoginResult(val success: Boolean, val log: String, val user: PacienteDto?)

    suspend fun login(nombre: String, email: String, esFamiliar: Boolean): LoginResult {
        var log = "Intentando login con Nombre=$nombre, Email=$email\n"
        return try {
            val payload = LoginPayload(nombre, email, esFamiliar)
            val response = api.login(payload)
            log += "Status: ${response.code()}\n"
            val bodyStr = response.body()
            log += "Respuesta: ${bodyStr}\n"
            if (response.isSuccessful && bodyStr != null && bodyStr.Success) {
                LoginResult(true, log, bodyStr.User)
            } else {
                LoginResult(false, log, null)
            }
        } catch (ex: Exception) {
            log += "Excepción: ${ex.message}\n"
            LoginResult(false, log, null)
        }
    }
}

