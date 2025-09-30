package com.mindraco.moodtab.data

import android.content.Context
import android.content.SharedPreferences
import androidx.security.crypto.EncryptedSharedPreferences
import androidx.security.crypto.MasterKey

object SecurePrefsHelper {
    private const val PREFS_NAME = "moodtab_secure_prefs"

    fun getEncryptedPrefs(context: Context): SharedPreferences {
        val masterKey = MasterKey.Builder(context)
            .setKeyScheme(MasterKey.KeyScheme.AES256_GCM)
            .build()
        return EncryptedSharedPreferences.create(
            context,
            PREFS_NAME,
            masterKey,
            EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
            EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
        )
    }

    fun saveLoginData(context: Context, user: PacienteDto, esFamiliar: Boolean) {
        val prefs = getEncryptedPrefs(context)
        prefs.edit()
            .putString("user_id", user.ID_Paciente.toString())
            .putString("user_nombre", user.Nombre)
            .putString("user_email", user.Email)
            .putString("externo_id", user.IdUsuarioExterno?.toString() ?: "0")
            .putBoolean("es_familiar", esFamiliar)
            .apply()
    }
}
